namespace SmartFactory.Api.Models.Requests;

/// <summary>
/// Payload for creating a new electronic form. FormType and Summary are required.
/// RequesterId is optional and defaults to the demo employee when omitted; the
/// approver is routed automatically from the form type.
/// </summary>
public sealed record CreateFormRequest(string? FormType, string? RequesterId, string? Summary);
