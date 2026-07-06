namespace SmartFactory.Api.Models.Database;

public sealed class FormApprovalStepEntity
{
    public string Id { get; set; } = string.Empty;
    public string FormId { get; set; } = string.Empty;
    public int StepOrder { get; set; }
    public string ApproverId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ActionAt { get; set; }
    public string? Note { get; set; }
}