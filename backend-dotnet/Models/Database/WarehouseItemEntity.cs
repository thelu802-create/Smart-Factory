namespace SmartFactory.Api.Models.Database;

public sealed class WarehouseItemEntity
{
    public string Id { get; set; } = string.Empty;
    public string IoId { get; set; } = string.Empty;
    public string IoCode { get; set; } = string.Empty;
    public string Bu { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string BatchCode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string ZoneId { get; set; } = string.Empty;
    public string Shelf { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string LastMovementAt { get; set; } = string.Empty;
}