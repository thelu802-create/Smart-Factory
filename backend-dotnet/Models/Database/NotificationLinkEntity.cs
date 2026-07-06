namespace SmartFactory.Api.Models.Database;

public sealed class NotificationLinkEntity
{
    public string NotificationId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string LinkRole { get; set; } = string.Empty;
}