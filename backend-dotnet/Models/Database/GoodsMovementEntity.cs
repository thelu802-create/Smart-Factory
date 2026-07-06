namespace SmartFactory.Api.Models.Database;

public sealed class GoodsMovementEntity
{
    public string Id { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string? FromZoneId { get; set; }
    public string ToZoneId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public string MovedBy { get; set; } = string.Empty;
    public string MovedAt { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}