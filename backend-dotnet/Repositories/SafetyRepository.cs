using Microsoft.EntityFrameworkCore;
using SmartFactory.Api.Data;
using SmartFactory.Api.Models;
using SmartFactory.Api.Models.Database;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// EF Core access for the safety module. Reads alerts on every request and
/// persists resolve/escalate actions (status + action_note).
/// </summary>
public sealed class SafetyRepository(SmartFactoryDbContext context)
{
    public bool IsAvailable() => context.Database.CanConnect();

    public IReadOnlyList<SafetyAlertDto> GetAlerts()
    {
        return (from alert in context.SafetyAlerts
                join area in context.FactoryAreas on alert.AreaId equals area.Id
                orderby alert.DetectedAt descending
                select new { alert, Location = area.Name })
            .AsEnumerable()
            .Select(row => ToDto(row.alert, row.Location))
            .ToList();
    }

    public SafetyAlertDto? GetAlert(string alertId)
    {
        var row = (from alert in context.SafetyAlerts
                   join area in context.FactoryAreas on alert.AreaId equals area.Id
                   where alert.Id == alertId
                   select new { alert, Location = area.Name })
            .FirstOrDefault();
        return row is null ? null : ToDto(row.alert, row.Location);
    }

    /// <summary>Marks an alert resolved. Returns the updated alert, or null when the id does not exist.</summary>
    public SafetyAlertDto? Resolve(string alertId, string? note) => Decide(alertId, "Resolved", note ?? "Resolved");

    /// <summary>Escalates an alert. Returns the updated alert, or null when the id does not exist.</summary>
    public SafetyAlertDto? Escalate(string alertId, string? note) => Decide(alertId, "Escalated", note ?? "Escalated");

    private SafetyAlertDto? Decide(string alertId, string status, string actionNote)
    {
        var alert = context.SafetyAlerts.FirstOrDefault(item => item.Id == alertId);
        if (alert is null)
        {
            return null;
        }

        alert.Status = status;
        alert.ActionNote = actionNote;
        context.SaveChanges();
        return GetAlert(alertId);
    }

    private static SafetyAlertDto ToDto(SafetyAlertEntity alert, string location) => new(
        alert.Id,
        alert.Title,
        alert.AlertType,
        alert.Severity,
        location,
        alert.Status,
        TimeValue(alert.DetectedAt),
        alert.Description);

    private static string TimeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains('T') && value.Length >= 16 ? value.Substring(11, 5) : value;
    }
}
