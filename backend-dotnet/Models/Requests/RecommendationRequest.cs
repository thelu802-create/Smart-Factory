namespace SmartFactory.Api.Models.Requests;

public sealed record RecommendationRequest(int TargetOutput, int AvailableWorkers);