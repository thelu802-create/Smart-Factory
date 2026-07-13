namespace SmartFactory.Api.Models;

public sealed record WarehouseZoneDto(string Id, string Name, string ZoneType, int Capacity, int CurrentUsage, string Status);

public sealed record GoodsMovementDto(string Id, string MovementType, int Quantity, string? FromZone, string ToZone, string MovedAt, string Note);
