namespace SmartFactory.Api.Models.Database;

public sealed class CameraEventEntity
{
    public string Id { get; set; } = string.Empty;
    public string CameraCode { get; set; } = string.Empty;
    public string AreaId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string EventTime { get; set; } = string.Empty;
    public string? RelatedAlertId { get; set; }
}