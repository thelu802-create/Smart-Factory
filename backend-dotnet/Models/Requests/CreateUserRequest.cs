namespace SmartFactory.Api.Models.Requests;

/// <summary>Payload for creating a user. FullName, Email, and RoleId are required.</summary>
public sealed record CreateUserRequest(string? FullName, string? Email, string? RoleId, string? Department);
