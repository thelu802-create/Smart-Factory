namespace SmartFactory.Api.Models.Requests;

/// <summary>Payload for approving/rejecting several forms in one request.</summary>
public sealed record BatchDecisionRequest(List<string>? Ids, string? Note);
