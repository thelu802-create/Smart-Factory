namespace SmartFactory.Api.Models.Database;

public sealed class ShiftPlanEntity
{
    public string Id { get; set; } = string.Empty;
    public string ShiftDate { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;
    public string LineId { get; set; } = string.Empty;
    public int RequiredWorkers { get; set; }
    public int AssignedWorkers { get; set; }
    public double OvertimeHours { get; set; }
    public string Status { get; set; } = string.Empty;
}