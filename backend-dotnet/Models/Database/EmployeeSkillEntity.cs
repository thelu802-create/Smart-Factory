namespace SmartFactory.Api.Models.Database;

public sealed class EmployeeSkillEntity
{
    public string EmployeeId { get; set; } = string.Empty;
    public string SkillId { get; set; } = string.Empty;
    public string ProficiencyLevel { get; set; } = string.Empty;
    public string? CertifiedAt { get; set; }
    public string? ExpiresAt { get; set; }
}