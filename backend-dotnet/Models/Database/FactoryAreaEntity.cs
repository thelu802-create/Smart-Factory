namespace SmartFactory.Api.Models.Database;

public sealed class FactoryAreaEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AreaType { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}