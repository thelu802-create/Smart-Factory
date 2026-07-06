namespace SmartFactory.Api.Models.Database;

public sealed class ProductionRecordEntity
{
    public string Id { get; set; } = string.Empty;
    public string LineId { get; set; } = string.Empty;
    public string RecordTime { get; set; } = string.Empty;
    public int TargetOutput { get; set; }
    public int ActualOutput { get; set; }
    public int DefectCount { get; set; }
    public int DowntimeMinutes { get; set; }
}