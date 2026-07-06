namespace SmartFactory.Api.Models.Database;

public sealed class ReportSourceLinkEntity
{
    public string ReportId { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string SourceId { get; set; } = string.Empty;
    public string IncludedAt { get; set; } = string.Empty;
}