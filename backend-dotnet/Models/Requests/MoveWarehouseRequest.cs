namespace SmartFactory.Api.Models.Requests;

/// <summary>Payload for a warehouse stock movement. MovementType is "Import" or "Export".</summary>
public sealed record MoveWarehouseRequest(string? MovementType, int Quantity, string? Note);
