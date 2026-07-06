namespace SmartFactory.Api.Models.Database;

public sealed class ProductionLineEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AreaId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TargetOutput { get; set; }
    public int ActualOutput { get; set; }
    public int Efficiency { get; set; }
    public double DefectRate { get; set; }
    public int DowntimeMinutes { get; set; }
    public int AssignedWorkers { get; set; }
    public string Issue { get; set; } = string.Empty;
}