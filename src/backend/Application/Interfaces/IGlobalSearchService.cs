using Application.DTOs.Requests.Search;
using Application.DTOs.Responses;
using Application.DTOs.Responses.Search;

namespace Application.Interfaces;

public interface IGlobalSearchService
{
    Task<ApiResponse<SearchResponse>> SearchAsync(
        SearchRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<SearchReindexResponse>> ReindexAllAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponse<SearchHealthResponse>> HealthAsync(
        CancellationToken cancellationToken = default);
}
