namespace SmartFactory.Api.Models.Database;

public sealed class SkillEntity
{
    public string Id { get; set; } = string.Empty;
    public string SkillCode { get; set; } = string.Empty;
    public string SkillName { get; set; } = string.Empty;
    public string SkillGroup { get; set; } = string.Empty;
}