namespace SmartFactory.Api.Models.Requests;

/// <summary>Payload for a warehouse stock movement. MovementType is "Import", "Export", or "Transfer" (ToZoneId required for Transfer).</summary>
public sealed record MoveWarehouseRequest(string? MovementType, int Quantity, string? ToZoneId, string? Note);
