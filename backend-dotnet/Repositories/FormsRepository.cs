using Microsoft.EntityFrameworkCore;
using SmartFactory.Api.Data;
using SmartFactory.Api.Models;
using SmartFactory.Api.Models.Database;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// EF Core access for the electronic forms module. Reads and writes on every
/// request so approve/reject/create actions are persisted and immediately visible.
/// </summary>
public sealed class FormsRepository(SmartFactoryDbContext context, AppSettingsService settings, WarehouseRepository warehouse)
{
    /// <summary>Result of a create/approve action: Status is "ok", "not_found", or "invalid".</summary>
    public sealed record FormActionResult(string Status, string? Error, FormRequestDto? Form);

    /// <summary>Per-form outcome inside a batch decision.</summary>
    public sealed record BatchItemResult(string Id, string Status, string? Error);

    public bool IsAvailable() => context.Database.CanConnect();

    /// <summary>
    /// Approves several forms — each in its own transaction, so one failure (e.g. a borrow
    /// whose stock is gone) does not roll back the others. Returns a per-form outcome.
    /// </summary>
    public IReadOnlyList<BatchItemResult> ApproveMany(IEnumerable<string> ids, string? note)
    {
        var results = new List<BatchItemResult>();
        foreach (var id in ids.Distinct())
        {
            var result = Approve(id, note);
            results.Add(new BatchItemResult(id, result.Status, result.Error));
        }

        return results;
    }

    /// <summary>Rejects several forms, one transaction each. Returns a per-form outcome.</summary>
    public IReadOnlyList<BatchItemResult> RejectMany(IEnumerable<string> ids, string? reason)
    {
        var results = new List<BatchItemResult>();
        foreach (var id in ids.Distinct())
        {
            var form = Reject(id, reason);
            results.Add(new BatchItemResult(id, form is null ? "not_found" : "ok", form is null ? "Form request not found" : null));
        }

        return results;
    }

    public IReadOnlyList<FormRequestDto> GetForms()
    {
        return (from form in context.FormRequests
                join user in context.Users on form.RequesterId equals user.Id
                orderby form.SubmittedAt descending
                select new { form, user.FullName, user.Department })
            .AsEnumerable()
            .Select(row => ToDto(row.form, row.FullName, row.Department))
            .ToList();
    }

    /// <summary>
    /// Approves a form. For "Warehouse Borrow" forms this also deducts the borrowed stock
    /// (an Export movement, with the warehouse auto-alerts) and notifies the requester —
    /// all in one transaction. Status: "ok", "not_found", or "invalid" (e.g. the stock is
    /// no longer available at approval time).
    /// </summary>
    public FormActionResult Approve(string formId, string? note)
    {
        var form = context.FormRequests.FirstOrDefault(item => item.Id == formId);
        if (form is null)
        {
            return new FormActionResult("not_found", "Form request not found", null);
        }

        using var transaction = context.Database.BeginTransaction();
        var actionAt = UtcNowIso();

        // Stock-borrow forms: withdraw the borrowed quantity from the warehouse.
        if (form.RelatedItemId is not null && form.RelatedQuantity is > 0)
        {
            var move = warehouse.ApplyMove(form.RelatedItemId, "Export", form.RelatedQuantity.Value, null, $"Warehouse borrow — form {formId}");
            if (move.Status != "ok")
            {
                return new FormActionResult("invalid", move.Error ?? "Could not deduct stock.", null); // rolls back on dispose
            }

            var itemCode = context.WarehouseItems.Where(w => w.Id == form.RelatedItemId).Select(w => w.ItemCode).FirstOrDefault() ?? form.RelatedItemId;
            context.Notifications.Add(new NotificationEntity
            {
                Id = "noti-" + Guid.NewGuid().ToString("N")[..8],
                Title = $"Borrow approved: {form.RelatedQuantity} x {itemCode}",
                NotificationType = "Warehouse",
                Severity = "Medium",
                Status = "Unread",
                TargetUserId = form.RequesterId,
                RelatedEntityType = "FormRequest",
                RelatedEntityId = formId,
                CreatedAt = actionAt
            });
        }

        form.Status = "Approved";
        form.ApprovedAt = actionAt;
        form.RejectionReason = null;

        foreach (var step in context.FormApprovalSteps.Where(step => step.FormId == formId))
        {
            step.Status = "Approved";
            step.ActionAt = actionAt;
            step.Note = note ?? "Approved";
        }

        context.SaveChanges();
        transaction.Commit();
        return new FormActionResult("ok", null, GetForm(formId));
    }

    /// <summary>Rejects a form. Returns the updated form, or null when the id does not exist.</summary>
    public FormRequestDto? Reject(string formId, string? reason) =>
        Decide(formId, "Rejected", approvedAt: null, rejectionReason: reason, stepNote: reason ?? "Rejected");

    /// <summary>
    /// Creates a new pending form, its first approval step, and an approver notification.
    /// Returns the created form, or null when the requester does not exist.
    /// </summary>
    public FormRequestDto? Create(string formType, string? requesterId, string summary)
    {
        var requester = string.IsNullOrWhiteSpace(requesterId) ? settings.GetString("forms.default_requester", "user-005") : requesterId!;
        if (!context.Users.Any(user => user.Id == requester))
        {
            return null;
        }

        var approverId = ResolveApprover(formType);
        var formId = "form-" + Guid.NewGuid().ToString("N")[..8];
        var submittedAt = UtcNowIso();
        var severity = formType.ToLowerInvariant().Contains("machine") ? "High" : "Medium";

        using var transaction = context.Database.BeginTransaction();

        context.FormRequests.Add(new FormRequestEntity
        {
            Id = formId,
            FormType = formType,
            RequesterId = requester,
            ApproverId = approverId,
            Status = "Pending Approval",
            SubmittedAt = submittedAt,
            ApprovedAt = null,
            Summary = summary,
            RejectionReason = null
        });

        context.FormApprovalSteps.Add(new FormApprovalStepEntity
        {
            Id = "step-" + Guid.NewGuid().ToString("N")[..8],
            FormId = formId,
            StepOrder = 1,
            ApproverId = approverId,
            Status = "Pending",
            ActionAt = null,
            Note = null
        });

        // Notify the routed approver so the new request appears in the Notifications inbox.
        context.Notifications.Add(new NotificationEntity
        {
            Id = "noti-" + Guid.NewGuid().ToString("N")[..8],
            Title = $"New {formType} pending approval",
            NotificationType = "Forms",
            Severity = severity,
            Status = "Unread",
            TargetUserId = approverId,
            RelatedEntityType = "FormRequest",
            RelatedEntityId = formId,
            CreatedAt = submittedAt
        });

        context.SaveChanges();
        transaction.Commit();

        return GetForm(formId);
    }

    /// <summary>
    /// Creates a "Warehouse Borrow" form. Validates the item exists and the quantity is
    /// available now; the stock is actually deducted later, on approval.
    /// </summary>
    public FormActionResult CreateStockBorrow(string? requesterId, string? itemId, int quantity, string? note)
    {
        var requester = string.IsNullOrWhiteSpace(requesterId) ? settings.GetString("forms.default_requester", "user-005") : requesterId!;
        if (!context.Users.Any(user => user.Id == requester))
        {
            return new FormActionResult("invalid", "Requester not found.", null);
        }

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return new FormActionResult("invalid", "itemId is required.", null);
        }

        if (quantity <= 0)
        {
            return new FormActionResult("invalid", "quantity must be greater than 0.", null);
        }

        var item = context.WarehouseItems.FirstOrDefault(w => w.Id == itemId);
        if (item is null)
        {
            return new FormActionResult("invalid", "Warehouse item not found.", null);
        }

        if (quantity > item.Quantity)
        {
            return new FormActionResult("invalid", $"Borrow quantity ({quantity}) exceeds available stock ({item.Quantity}).", null);
        }

        const string formType = "Warehouse Borrow";
        var approverId = ResolveApprover(formType);
        var formId = "form-" + Guid.NewGuid().ToString("N")[..8];
        var submittedAt = UtcNowIso();
        var summary = $"Borrow {quantity} x {item.ItemCode} ({item.ItemName})"
            + (string.IsNullOrWhiteSpace(note) ? string.Empty : $" — {note}");

        using var transaction = context.Database.BeginTransaction();

        context.FormRequests.Add(new FormRequestEntity
        {
            Id = formId,
            FormType = formType,
            RequesterId = requester,
            ApproverId = approverId,
            Status = "Pending Approval",
            SubmittedAt = submittedAt,
            ApprovedAt = null,
            Summary = summary,
            RejectionReason = null,
            RelatedItemId = itemId,
            RelatedQuantity = quantity
        });

        context.FormApprovalSteps.Add(new FormApprovalStepEntity
        {
            Id = "step-" + Guid.NewGuid().ToString("N")[..8],
            FormId = formId,
            StepOrder = 1,
            ApproverId = approverId,
            Status = "Pending",
            ActionAt = null,
            Note = null
        });

        context.Notifications.Add(new NotificationEntity
        {
            Id = "noti-" + Guid.NewGuid().ToString("N")[..8],
            Title = $"Warehouse borrow pending approval: {quantity} x {item.ItemCode}",
            NotificationType = "Forms",
            Severity = "Medium",
            Status = "Unread",
            TargetUserId = approverId,
            RelatedEntityType = "FormRequest",
            RelatedEntityId = formId,
            CreatedAt = submittedAt
        });

        context.SaveChanges();
        transaction.Commit();
        return new FormActionResult("ok", null, GetForm(formId));
    }

    private FormRequestDto? Decide(string formId, string status, string? approvedAt, string? rejectionReason, string stepNote)
    {
        var form = context.FormRequests.FirstOrDefault(item => item.Id == formId);
        if (form is null)
        {
            return null;
        }

        using var transaction = context.Database.BeginTransaction();

        form.Status = status;
        form.ApprovedAt = approvedAt;
        form.RejectionReason = rejectionReason;

        // Advance the approval workflow steps for this form so form_approval_steps stays in sync.
        var actionAt = UtcNowIso();
        foreach (var step in context.FormApprovalSteps.Where(step => step.FormId == formId))
        {
            step.Status = status;
            step.ActionAt = actionAt;
            step.Note = stepNote;
        }

        context.SaveChanges();
        transaction.Commit();

        return GetForm(formId);
    }

    private FormRequestDto? GetForm(string formId)
    {
        var row = (from form in context.FormRequests
                   join user in context.Users on form.RequesterId equals user.Id
                   where form.Id == formId
                   select new { form, user.FullName, user.Department })
            .FirstOrDefault();
        return row is null ? null : ToDto(row.form, row.FullName, row.Department);
    }

    // Routes a new form to the responsible approver based on its type (target user ids
    // configurable via app_settings, falling back to the seeded managers).
    private string ResolveApprover(string formType)
    {
        var type = formType.ToLowerInvariant();
        if (type.Contains("machine")) return settings.GetString("forms.approver.machine", "user-002");                          // Production Manager
        if (type.Contains("warehouse") || type.Contains("import") || type.Contains("export")) return settings.GetString("forms.approver.warehouse", "user-003"); // Warehouse Manager
        return settings.GetString("forms.approver.default", "user-001");                                                        // Factory Manager (leave, overtime, default)
    }

    private static FormRequestDto ToDto(FormRequestEntity form, string requester, string department) => new(
        form.Id,
        form.FormType,
        requester,
        department,
        form.Status,
        TimeValue(form.SubmittedAt),
        form.Summary);

    private static string UtcNowIso() => DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

    private static string TimeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains('T') && value.Length >= 16 ? value.Substring(11, 5) : value;
    }
}
