namespace Application.DTOs.Responses.Search;

public sealed class SearchHealthResponse
{
    public bool IsEnabled { get; set; }
    public bool IsAvailable { get; set; }
    public string Engine { get; set; } = "Elasticsearch";
    public string IndexName { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? ErrorMessage { get; set; }
}
