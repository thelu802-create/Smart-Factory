namespace SmartFactory.Api.Models.Database;

public sealed class AiRecommendationLinkEntity
{
    public string RecommendationId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string LinkReason { get; set; } = string.Empty;
}