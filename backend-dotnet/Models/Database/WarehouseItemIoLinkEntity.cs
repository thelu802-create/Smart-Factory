namespace SmartFactory.Api.Models.Database;

public sealed class WarehouseItemIoLinkEntity
{
    public string ItemId { get; set; } = string.Empty;
    public string IoMasterId { get; set; } = string.Empty;
    public string LinkType { get; set; } = string.Empty;
    public string EffectiveFrom { get; set; } = string.Empty;
    public string? EffectiveTo { get; set; }
}