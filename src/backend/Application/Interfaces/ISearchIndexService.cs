using Application.DTOs.Requests.Search;
using Application.DTOs.Responses.Search;

namespace Application.Interfaces;

public interface ISearchIndexService
{
    bool IsEnabled { get; }
    string IndexName { get; }

    Task EnsureIndexAsync(CancellationToken cancellationToken = default);

    Task<SearchResponse> SearchAsync(
        SearchRequest request,
        CancellationToken cancellationToken = default);

    Task BulkUpsertAsync(
        IReadOnlyCollection<SearchDocument> documents,
        CancellationToken cancellationToken = default);

    Task UpsertAsync(
        SearchDocument document,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string documentId,
        CancellationToken cancellationToken = default);

    Task<SearchHealthResponse> HealthAsync(CancellationToken cancellationToken = default);
}
