namespace SmartFactory.Api.Models.Database;

public sealed class ShiftPlanAssignmentEntity
{
    public string ShiftId { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string AssignmentRole { get; set; } = string.Empty;
    public string AssignmentStatus { get; set; } = string.Empty;
    public string AssignedAt { get; set; } = string.Empty;
}