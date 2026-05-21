namespace Application.DTOs.Responses.Search;

public sealed class SearchResponse
{
    public string Query { get; set; } = string.Empty;
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long Total { get; set; }
    public bool IsFromElasticsearch { get; set; }
    public string Engine { get; set; } = string.Empty;
    public TimeSpan Took { get; set; }
    public List<SearchHitResponse> Items { get; set; } = new();
}

public sealed class SearchHitResponse
{
    public double Score { get; set; }
    public SearchDocument Document { get; set; } = new();
}
