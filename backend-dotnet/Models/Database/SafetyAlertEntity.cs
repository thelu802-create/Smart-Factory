namespace SmartFactory.Api.Models.Database;

public sealed class SafetyAlertEntity
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string AreaId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string DetectedAt { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ActionNote { get; set; }
}