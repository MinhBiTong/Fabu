namespace Application.DTOs.Responses.Search;

public sealed class SearchDocument
{
    public string Id { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new();
    public long? CustomerId { get; set; }
    public long? UserId { get; set; }
    public decimal? Amount { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Dictionary<string, string?> Metadata { get; set; } = new();
}
