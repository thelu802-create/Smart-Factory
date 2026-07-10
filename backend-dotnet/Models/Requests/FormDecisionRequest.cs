namespace SmartFactory.Api.Models.Requests;

/// <summary>Optional payload for approve/reject actions. Note becomes the approval-step note or rejection reason.</summary>
public sealed record FormDecisionRequest(string? Note);
