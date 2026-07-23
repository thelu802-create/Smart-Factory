using Microsoft.EntityFrameworkCore;
using SmartFactory.Api.Data;
using SmartFactory.Api.Models;
using SmartFactory.Api.Models.Database;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// EF Core access for the workforce module. The shift planner matches actual
/// available employees (by line affinity / skill tags) to under-staffed shifts and
/// writes named staffing recommendations into ai_recommendations.
/// </summary>
public sealed class WorkforceRepository(SmartFactoryDbContext context)
{
    // Auto-generated rows are marked with this id prefix so regenerating replaces
    // only our own rows and never touches seeded/demo recommendations.
    private const string AutoPrefix = "wf-auto-";

    public bool IsAvailable() => context.Database.CanConnect();

    public IReadOnlyList<RecommendationDto> GetRecommendations() => ReadRecommendations();

    /// <summary>
    /// Recomputes staffing recommendations. For each shift where assigned &lt; required,
    /// it picks available employees suited to that line (already on the line, or whose
    /// skill tags mention it) that are not yet assigned to the shift, and names them.
    /// Replaces any previously auto-generated rows. Returns the full message list.
    /// </summary>
    public IReadOnlyList<RecommendationDto> GenerateRecommendations()
    {
        using var transaction = context.Database.BeginTransaction();

        // Clear previously auto-generated recommendations so repeated clicks don't stack up.
        context.AiRecommendations.RemoveRange(context.AiRecommendations.Where(item => item.Id.StartsWith(AutoPrefix)));

        var createdAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        var gaps = ReadShiftGaps();

        if (gaps.Count == 0)
        {
            context.AiRecommendations.Add(NewRecommendation(
                "Staffing balanced",
                "All shifts currently meet their required worker count.",
                "No staffing action needed right now.",
                createdAt));
        }
        else
        {
            var employees = context.Employees
                .Select(e => new Candidate(e.Id, e.EmployeeCode, e.FullName, e.SkillTags, e.AvailabilityStatus, e.CurrentLineId))
                .ToList();
            var assignments = context.ShiftPlanAssignments
                .Select(a => new { a.ShiftId, a.EmployeeId })
                .ToList();

            foreach (var gap in gaps)
            {
                var need = gap.Required - gap.Assigned;
                var assignedSet = assignments.Where(a => a.ShiftId == gap.ShiftId).Select(a => a.EmployeeId).ToHashSet();

                var candidates = employees
                    .Where(e => string.Equals(e.Availability, "Available", StringComparison.OrdinalIgnoreCase)
                                && !assignedSet.Contains(e.Id)
                                && SuitsLine(e.CurrentLineId, e.SkillTags, gap.LineId, gap.LineName))
                    .OrderByDescending(e => e.CurrentLineId == gap.LineId) // already on this line first
                    .ThenBy(e => e.FullName)
                    .Take(need)
                    .ToList();

                context.AiRecommendations.Add(BuildGapRecommendation(gap, need, candidates, createdAt));
            }
        }

        context.SaveChanges();
        transaction.Commit();
        return ReadRecommendations();
    }

    /// <summary>Outcome of applying the plan: how many were assigned + the refreshed advisory list.</summary>
    public sealed record ApplyResult(int Assigned, IReadOnlyList<RecommendationDto> Recommendations);

    /// <summary>
    /// Actually staffs the under-filled shifts: for each gap it re-selects available,
    /// suited employees (revalidating against the current state, not the stored text),
    /// inserts shift_plan_assignments, bumps assigned_workers, and marks those employees
    /// Busy so they are never double-booked. All in one transaction. Then it refreshes
    /// the advisory recommendations to reflect what is still short.
    /// </summary>
    public ApplyResult ApplyRecommendations()
    {
        var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        var assignedTotal = 0;
        string? anyShiftId = null;

        using (var transaction = context.Database.BeginTransaction())
        {
            var lineNames = context.ProductionLines.ToDictionary(line => line.Id, line => line.Name);
            var gapShifts = context.ShiftPlans
                .Where(shift => shift.AssignedWorkers < shift.RequiredWorkers)
                .OrderByDescending(shift => shift.RequiredWorkers - shift.AssignedWorkers)
                .ToList();
            var employees = context.Employees.ToList(); // tracked — marking Busy prevents double-booking
            var existing = context.ShiftPlanAssignments.Select(a => new { a.ShiftId, a.EmployeeId }).ToList();

            foreach (var shift in gapShifts)
            {
                anyShiftId ??= shift.Id;
                var lineName = lineNames.TryGetValue(shift.LineId, out var name) ? name : shift.LineId;
                var need = shift.RequiredWorkers - shift.AssignedWorkers;
                var alreadyInShift = existing.Where(a => a.ShiftId == shift.Id).Select(a => a.EmployeeId).ToHashSet();

                var chosen = employees
                    .Where(e => string.Equals(e.AvailabilityStatus, "Available", StringComparison.OrdinalIgnoreCase)
                                && !alreadyInShift.Contains(e.Id)
                                && SuitsLine(e.CurrentLineId, e.SkillTags, shift.LineId, lineName))
                    .OrderByDescending(e => e.CurrentLineId == shift.LineId)
                    .ThenBy(e => e.FullName)
                    .Take(need)
                    .ToList();

                foreach (var employee in chosen)
                {
                    context.ShiftPlanAssignments.Add(new ShiftPlanAssignmentEntity
                    {
                        ShiftId = shift.Id,
                        EmployeeId = employee.Id,
                        AssignmentRole = "Operator",
                        AssignmentStatus = "Assigned",
                        AssignedAt = now
                    });
                    employee.AvailabilityStatus = "Busy";
                }

                if (chosen.Count > 0)
                {
                    shift.AssignedWorkers += chosen.Count;
                    if (shift.AssignedWorkers >= shift.RequiredWorkers)
                    {
                        shift.Status = "Published";
                    }

                    assignedTotal += chosen.Count;
                }
            }

            if (assignedTotal > 0)
            {
                context.Notifications.Add(new NotificationEntity
                {
                    Id = "noti-" + Guid.NewGuid().ToString("N")[..8],
                    Title = $"Assigned {assignedTotal} employees to understaffed shifts",
                    NotificationType = "Workforce",
                    Severity = "Medium",
                    Status = "Unread",
                    TargetUserId = ProductionManagerId(),
                    RelatedEntityType = "ShiftPlan",
                    RelatedEntityId = anyShiftId ?? "shift",
                    CreatedAt = now
                });
            }

            context.SaveChanges();
            transaction.Commit();
        }

        // Refresh the advisory list against the post-apply state (own transaction).
        var messages = GenerateRecommendations();
        return new ApplyResult(assignedTotal, messages);
    }

    private string ProductionManagerId() =>
        context.Users.Where(user => user.Department == "Production").OrderBy(user => user.Id).Select(user => user.Id).FirstOrDefault()
            ?? context.Users.OrderBy(user => user.Id).Select(user => user.Id).FirstOrDefault()
            ?? "user-002";

    // An employee suits a line if they already work it, or a skill tag mentions its name.
    private static bool SuitsLine(string? currentLineId, string skillTags, string lineId, string lineName)
    {
        if (currentLineId == lineId)
        {
            return true;
        }

        return skillTags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(tag => tag.Contains(lineName, StringComparison.OrdinalIgnoreCase));
    }

    private AiRecommendationEntity BuildGapRecommendation(ShiftGap gap, int need, List<Candidate> candidates, string createdAt)
    {
        var title = $"Assign {need} to {gap.LineName} - {gap.Shift}";
        var reason = $"Needs {gap.Required}, assigned {gap.Assigned} (short {need}).";

        string impact;
        if (candidates.Count == 0)
        {
            impact = $"No available employees suited to {gap.LineName} — consider overtime or transferring from another line.";
        }
        else
        {
            var names = string.Join(", ", candidates.Select(c => $"{c.Code} {c.FullName}"));
            impact = candidates.Count >= need
                ? $"Suggested: {names}."
                : $"Suggested: {names}; still short {need - candidates.Count} — consider overtime.";
        }

        return NewRecommendation(title, reason, impact, createdAt);
    }

    private static AiRecommendationEntity NewRecommendation(string title, string reason, string expectedImpact, string createdAt) => new()
    {
        Id = AutoPrefix + Guid.NewGuid().ToString("N"),
        Module = "Workforce",
        Title = title,
        Reason = reason,
        ExpectedImpact = expectedImpact,
        Status = "New",
        CreatedAt = createdAt,
        ReviewedBy = null
    };

    private sealed record Candidate(string Id, string Code, string FullName, string SkillTags, string Availability, string? CurrentLineId);
    private sealed record ShiftGap(string ShiftId, string Shift, string LineId, string LineName, int Required, int Assigned);

    private List<ShiftGap> ReadShiftGaps()
    {
        return (from shift in context.ShiftPlans
                join line in context.ProductionLines on shift.LineId equals line.Id
                where shift.AssignedWorkers < shift.RequiredWorkers
                orderby (shift.RequiredWorkers - shift.AssignedWorkers) descending
                select new ShiftGap(shift.Id, shift.ShiftName, line.Id, line.Name, shift.RequiredWorkers, shift.AssignedWorkers))
            .ToList();
    }

    private List<RecommendationDto> ReadRecommendations()
    {
        return context.AiRecommendations
            .Where(item => item.Module == "Workforce")
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new RecommendationDto(item.Title, item.ExpectedImpact))
            .ToList();
    }
}
