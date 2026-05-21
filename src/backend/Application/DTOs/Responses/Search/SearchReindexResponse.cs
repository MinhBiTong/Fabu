namespace Application.DTOs.Responses.Search;

public sealed class SearchReindexResponse
{
    public bool IsSuccess { get; set; }
    public int IndexedCount { get; set; }
    public string IndexName { get; set; } = string.Empty;
    public TimeSpan Took { get; set; }
    public string? ErrorMessage { get; set; }
}
