using System.Diagnostics;
using Application.DTOs.Requests.Search;
using Application.DTOs.Responses;
using Application.DTOs.Responses.Search;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.Search;

public sealed class GlobalSearchService : IGlobalSearchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISearchIndexService _searchIndex;
    private readonly ISearchDocumentMapper _mapper;
    private readonly ElasticsearchConfiguration _configuration;
    private readonly ILogger<GlobalSearchService> _logger;

    public GlobalSearchService(
        IUnitOfWork unitOfWork,
        ISearchIndexService searchIndex,
        ISearchDocumentMapper mapper,
        IOptions<ElasticsearchConfiguration> options,
        ILogger<GlobalSearchService> logger)
    {
        _unitOfWork = unitOfWork;
        _searchIndex = searchIndex;
        _mapper = mapper;
        _configuration = options.Value;
        _logger = logger;
    }

    public async Task<ApiResponse<SearchResponse>> SearchAsync(
        SearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(request);

        if (_searchIndex.IsEnabled)
        {
            try
            {
                var elasticResponse = await _searchIndex.SearchAsync(normalized, cancellationToken);
                return ApiResponse<SearchResponse>.Success(elasticResponse);
            }
            catch (Exception ex) when (_configuration.UseDatabaseFallback)
            {
                _logger.LogWarning(ex, "Elasticsearch search failed. Falling back to database search.");
            }
        }

        if (!_configuration.UseDatabaseFallback && _searchIndex.IsEnabled)
        {
            return ApiResponse<SearchResponse>.Fail(503, "Elasticsearch is unavailable.");
        }

        var fallback = await SearchDatabaseAsync(normalized, cancellationToken);
        return ApiResponse<SearchResponse>.Success(fallback, "Search completed using database fallback.");
    }

    public async Task<ApiResponse<SearchReindexResponse>> ReindexAllAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        if (!_searchIndex.IsEnabled)
        {
            return ApiResponse<SearchReindexResponse>.Fail(400, "Elasticsearch is disabled.");
        }

        try
        {
            await _searchIndex.EnsureIndexAsync(cancellationToken);
            var documents = await BuildAllDocumentsAsync(cancellationToken);
            await _searchIndex.BulkUpsertAsync(documents, cancellationToken);

            stopwatch.Stop();
            return ApiResponse<SearchReindexResponse>.Success(new SearchReindexResponse
            {
                IsSuccess = true,
                IndexedCount = documents.Count,
                IndexName = _searchIndex.IndexName,
                Took = stopwatch.Elapsed
            }, "Reindex completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Global Elasticsearch reindex failed.");
            stopwatch.Stop();
            return ApiResponse<SearchReindexResponse>.Fail(500, "Reindex failed.");
        }
    }

    public async Task<ApiResponse<SearchHealthResponse>> HealthAsync(CancellationToken cancellationToken = default)
    {
        var health = await _searchIndex.HealthAsync(cancellationToken);
        return ApiResponse<SearchHealthResponse>.Success(health);
    }

    private async Task<SearchResponse> SearchDatabaseAsync(
        SearchRequest request,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var allDocuments = await BuildAllDocumentsAsync(cancellationToken);
        var filtered = ApplyFilters(allDocuments, request).ToList();
        var total = filtered.Count;
        var items = filtered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(document => new SearchHitResponse
            {
                Score = CalculateFallbackScore(document, request.Query),
                Document = document
            })
            .ToList();

        stopwatch.Stop();
        return new SearchResponse
        {
            Query = request.Query ?? string.Empty,
            Page = request.Page,
            PageSize = request.PageSize,
            Total = total,
            IsFromElasticsearch = false,
            Engine = "DatabaseFallback",
            Took = stopwatch.Elapsed,
            Items = items
        };
    }

    private async Task<List<SearchDocument>> BuildAllDocumentsAsync(CancellationToken cancellationToken)
    {
        var entities = new List<object>();

        entities.AddRange(await _unitOfWork.Users.GetAllAsync());
        entities.AddRange(await _unitOfWork.Customers.GetAllAsync());
        entities.AddRange(await _unitOfWork.Accounts.GetAllAsync());
        entities.AddRange(await _unitOfWork.RechargePlans.GetAllAsync());
        entities.AddRange(await _unitOfWork.Services.GetAllAsync());
        entities.AddRange(await _unitOfWork.Transactions.GetAllAsync());
        entities.AddRange(await _unitOfWork.Payments.GetAllAsync());
        entities.AddRange(await _unitOfWork.PostpaidBills.GetAllAsync());
        entities.AddRange(await _unitOfWork.Coupons.GetAllAsync());
        entities.AddRange(await _unitOfWork.Feedbacks.GetAllAsync());
        entities.AddRange(await _unitOfWork.AuditLogs.GetAllAsync());
        entities.AddRange(await _unitOfWork.CustomerServices.GetAllAsync());

        cancellationToken.ThrowIfCancellationRequested();

        return entities
            .Select(entity => _mapper.Map(entity))
            .Where(document => document is not null)
            .Cast<SearchDocument>()
            .ToList();
    }

    private static SearchRequest Normalize(SearchRequest request)
    {
        return new SearchRequest
        {
            Query = request.Query?.Trim(),
            EntityTypes = request.EntityTypes
                .Where(type => !string.IsNullOrWhiteSpace(type))
                .Select(type => type.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList(),
            CustomerId = request.CustomerId,
            Status = request.Status?.Trim(),
            MinAmount = request.MinAmount,
            MaxAmount = request.MaxAmount,
            From = request.From,
            To = request.To,
            Page = Math.Max(1, request.Page),
            PageSize = Math.Clamp(request.PageSize, 1, 100),
            SortBy = string.IsNullOrWhiteSpace(request.SortBy) ? "score" : request.SortBy.Trim(),
            SortDescending = request.SortDescending
        };
    }

    private static IEnumerable<SearchDocument> ApplyFilters(IEnumerable<SearchDocument> documents, SearchRequest request)
    {
        var query = documents;

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var term = request.Query.Trim();
            query = query.Where(document =>
                Contains(document.Title, term) ||
                Contains(document.Summary, term) ||
                Contains(document.Content, term) ||
                document.Keywords.Any(keyword => Contains(keyword, term)));
        }

        if (request.EntityTypes.Count > 0)
        {
            query = query.Where(document => request.EntityTypes.Contains(document.EntityType, StringComparer.OrdinalIgnoreCase));
        }

        if (request.CustomerId.HasValue)
        {
            query = query.Where(document => document.CustomerId == request.CustomerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(document => string.Equals(document.Status, request.Status, StringComparison.OrdinalIgnoreCase));
        }

        if (request.MinAmount.HasValue)
        {
            query = query.Where(document => document.Amount >= request.MinAmount.Value);
        }

        if (request.MaxAmount.HasValue)
        {
            query = query.Where(document => document.Amount <= request.MaxAmount.Value);
        }

        if (request.From.HasValue)
        {
            query = query.Where(document => document.CreatedAt >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(document => document.CreatedAt <= request.To.Value);
        }

        return Sort(query, request);
    }

    private static IEnumerable<SearchDocument> Sort(IEnumerable<SearchDocument> documents, SearchRequest request)
    {
        return request.SortBy.ToLowerInvariant() switch
        {
            "amount" => request.SortDescending
                ? documents.OrderByDescending(document => document.Amount ?? 0)
                : documents.OrderBy(document => document.Amount ?? 0),
            "createdat" or "created" => request.SortDescending
                ? documents.OrderByDescending(document => document.CreatedAt ?? DateTimeOffset.MinValue)
                : documents.OrderBy(document => document.CreatedAt ?? DateTimeOffset.MinValue),
            "title" => request.SortDescending
                ? documents.OrderByDescending(document => document.Title)
                : documents.OrderBy(document => document.Title),
            _ => documents.OrderByDescending(document => CalculateFallbackScore(document, request.Query))
        };
    }

    private static double CalculateFallbackScore(SearchDocument document, string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return 1;
        }

        var term = query.Trim();
        var score = 0d;
        if (Contains(document.Title, term)) score += 5;
        if (Contains(document.Summary, term)) score += 3;
        if (Contains(document.Content, term)) score += 1;
        score += document.Keywords.Count(keyword => Contains(keyword, term)) * 0.5;
        return score;
    }

    private static bool Contains(string? value, string term)
        => !string.IsNullOrWhiteSpace(value) &&
           value.Contains(term, StringComparison.OrdinalIgnoreCase);
}
