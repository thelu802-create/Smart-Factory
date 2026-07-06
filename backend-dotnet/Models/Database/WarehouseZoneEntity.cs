namespace SmartFactory.Api.Models.Database;

public sealed class WarehouseZoneEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ZoneType { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int CurrentUsage { get; set; }
    public string Status { get; set; } = string.Empty;
}