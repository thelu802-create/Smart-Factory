namespace SmartFactory.Api.Models.Database;

public sealed class MachineEntity
{
    public string Id { get; set; } = string.Empty;
    public string MachineCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LineId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string LastMaintenanceAt { get; set; } = string.Empty;
}