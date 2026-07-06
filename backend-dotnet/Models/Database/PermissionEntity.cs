namespace SmartFactory.Api.Models.Database;

public sealed class PermissionEntity
{
    public string Id { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}