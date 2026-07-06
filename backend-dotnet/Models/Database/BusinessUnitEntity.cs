namespace SmartFactory.Api.Models.Database;

public sealed class BusinessUnitEntity
{
    public string Id { get; set; } = string.Empty;
    public string BuCode { get; set; } = string.Empty;
    public string BuName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}