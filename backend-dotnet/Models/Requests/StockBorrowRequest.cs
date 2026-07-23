namespace SmartFactory.Api.Models.Requests;

/// <summary>
/// Payload for creating a "Warehouse Borrow" form. ItemId and Quantity are required;
/// RequesterId defaults to the demo employee when omitted.
/// </summary>
public sealed record StockBorrowRequest(string? RequesterId, string? ItemId, int? Quantity, string? Note);
