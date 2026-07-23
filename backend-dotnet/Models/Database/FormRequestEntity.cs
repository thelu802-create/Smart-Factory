namespace SmartFactory.Api.Models.Database;

public sealed class FormRequestEntity
{
    public string Id { get; set; } = string.Empty;
    public string FormType { get; set; } = string.Empty;
    public string RequesterId { get; set; } = string.Empty;
    public string ApproverId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string SubmittedAt { get; set; } = string.Empty;
    public string? ApprovedAt { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }

    // Set for "Warehouse Borrow" forms: the stock that is auto-deducted on approval.
    public string? RelatedItemId { get; set; }
    public int? RelatedQuantity { get; set; }
}