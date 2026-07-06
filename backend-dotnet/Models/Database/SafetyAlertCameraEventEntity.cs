namespace SmartFactory.Api.Models.Database;

public sealed class SafetyAlertCameraEventEntity
{
    public string AlertId { get; set; } = string.Empty;
    public string CameraEventId { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
    public string LinkedAt { get; set; } = string.Empty;
}