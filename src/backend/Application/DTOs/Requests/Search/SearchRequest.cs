namespace Application.DTOs.Requests.Search;

public sealed class SearchRequest
{
    public string? Query { get; set; }
    public List<string> EntityTypes { get; set; } = new();
    public long? CustomerId { get; set; }
    public string? Status { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "score";
    public bool SortDescending { get; set; } = true;
}
