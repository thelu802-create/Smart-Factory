namespace SmartFactory.Api.Models.Database;

public sealed class RolePermissionEntity
{
    public string RoleId { get; set; } = string.Empty;
    public string PermissionId { get; set; } = string.Empty;
    public string GrantedAt { get; set; } = string.Empty;
}