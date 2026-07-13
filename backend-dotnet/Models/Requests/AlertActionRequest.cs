namespace SmartFactory.Api.Models.Requests;

/// <summary>Optional payload for safety alert resolve/escalate actions. Note is stored as action_note.</summary>
public sealed record AlertActionRequest(string? Note);
