using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.DTOs.Requests.Search;
using Application.DTOs.Responses.Search;
using Application.Interfaces;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Search;

public sealed class ElasticsearchSearchIndexService : ISearchIndexService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly ElasticsearchConfiguration _configuration;
    private readonly ILogger<ElasticsearchSearchIndexService> _logger;
    private volatile bool _indexEnsured;

    public ElasticsearchSearchIndexService(
        HttpClient httpClient,
        IOptions<ElasticsearchConfiguration> options,
        ILogger<ElasticsearchSearchIndexService> logger)
    {
        _httpClient = httpClient;
        _configuration = options.Value;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(Math.Clamp(_configuration.TimeoutSeconds, 3, 60));
        ConfigureClient();
    }

    public bool IsEnabled => _configuration.Enabled;
    public string IndexName => _configuration.IndexName;

    public async Task EnsureIndexAsync(CancellationToken cancellationToken = default)
    {
        if (!IsEnabled || _indexEnsured)
        {
            return;
        }

        using var head = new HttpRequestMessage(HttpMethod.Head, BuildUrl(IndexName));
        using var headResponse = await _httpClient.SendAsync(head, cancellationToken);
        if (headResponse.IsSuccessStatusCode)
        {
            _indexEnsured = true;
            return;
        }

        if (headResponse.StatusCode != HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException($"Elasticsearch index check failed: {(int)headResponse.StatusCode}.");
        }

        var payload = new
        {
            settings = new
            {
                number_of_shards = Math.Max(1, _configuration.NumberOfShards),
                number_of_replicas = Math.Max(0, _configuration.NumberOfReplicas),
                analysis = new
                {
                    filter = new
                    {
                        fabu_ascii = new
                        {
                            type = "asciifolding",
                            preserve_original = true
                        }
                    },
                    analyzer = new
                    {
                        fabu_text = new
                        {
                            tokenizer = "standard",
                            filter = new[] { "lowercase", "fabu_ascii" }
                        }
                    }
                }
            },
            mappings = new
            {
                dynamic = true,
                properties = new Dictionary<string, object>
                {
                    ["id"] = new { type = "keyword" },
                    ["entityType"] = new { type = "keyword" },
                    ["entityId"] = new { type = "keyword" },
                    ["title"] = new
                    {
                        type = "text",
                        analyzer = "fabu_text",
                        fields = new Dictionary<string, object>
                        {
                            ["keyword"] = new { type = "keyword", ignore_above = 256 }
                        }
                    },
                    ["summary"] = new { type = "text", analyzer = "fabu_text" },
                    ["content"] = new { type = "text", analyzer = "fabu_text" },
                    ["keywords"] = new { type = "keyword" },
                    ["customerId"] = new { type = "long" },
                    ["userId"] = new { type = "long" },
                    ["amount"] = new { type = "double" },
                    ["status"] = new { type = "keyword" },
                    ["createdAt"] = new { type = "date" },
                    ["updatedAt"] = new { type = "date" },
                    ["metadata"] = new { type = "object", enabled = false }
                }
            }
        };

        using var put = new HttpRequestMessage(HttpMethod.Put, BuildUrl(IndexName))
        {
            Content = ToJsonContent(payload)
        };
        using var putResponse = await _httpClient.SendAsync(put, cancellationToken);
        var body = await putResponse.Content.ReadAsStringAsync(cancellationToken);
        if (!putResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Create Elasticsearch index failed. Status: {Status}, Body: {Body}", (int)putResponse.StatusCode, Clip(body));
            throw new InvalidOperationException($"Create Elasticsearch index failed: {(int)putResponse.StatusCode}.");
        }

        _indexEnsured = true;
    }

    public async Task<SearchResponse> SearchAsync(
        SearchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!IsEnabled)
        {
            throw new InvalidOperationException("Elasticsearch is disabled.");
        }

        await EnsureIndexAsync(cancellationToken);
        var stopwatch = Stopwatch.StartNew();
        var payload = BuildSearchPayload(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, BuildUrl($"{IndexName}/_search"))
        {
            Content = ToJsonContent(payload)
        };

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Elasticsearch search failed. Status: {Status}, Body: {Body}", (int)response.StatusCode, Clip(body));
            throw new InvalidOperationException($"Elasticsearch search failed: {(int)response.StatusCode}.");
        }

        stopwatch.Stop();
        return ParseSearchResponse(body, request, stopwatch.Elapsed);
    }

    public async Task BulkUpsertAsync(
        IReadOnlyCollection<SearchDocument> documents,
        CancellationToken cancellationToken = default)
    {
        if (!IsEnabled || documents.Count == 0)
        {
            return;
        }

        await EnsureIndexAsync(cancellationToken);
        var bulkSize = Math.Clamp(_configuration.MaxBulkSize, 1, 2000);

        foreach (var chunk in documents.Chunk(bulkSize))
        {
            var builder = new StringBuilder();
            foreach (var document in chunk)
            {
                builder.Append(JsonSerializer.Serialize(new
                {
                    index = new
                    {
                        _index = IndexName,
                        _id = document.Id
                    }
                }, SerializerOptions));
                builder.Append('\n');
                builder.Append(JsonSerializer.Serialize(document, SerializerOptions));
                builder.Append('\n');
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl("_bulk"))
            {
                Content = new StringContent(builder.ToString(), Encoding.UTF8, "application/x-ndjson")
            };

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode || HasBulkErrors(body))
            {
                _logger.LogError("Elasticsearch bulk upsert failed. Status: {Status}, Body: {Body}", (int)response.StatusCode, Clip(body));
                throw new InvalidOperationException("Elasticsearch bulk upsert failed.");
            }
        }
    }

    public async Task UpsertAsync(SearchDocument document, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled || string.IsNullOrWhiteSpace(document.Id))
        {
            return;
        }

        await EnsureIndexAsync(cancellationToken);
        using var request = new HttpRequestMessage(
            HttpMethod.Put,
            BuildUrl($"{IndexName}/_doc/{Uri.EscapeDataString(document.Id)}"))
        {
            Content = ToJsonContent(document)
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Elasticsearch upsert failed. DocumentId: {DocumentId}, Status: {Status}, Body: {Body}", document.Id, (int)response.StatusCode, Clip(body));
            throw new InvalidOperationException($"Elasticsearch upsert failed: {(int)response.StatusCode}.");
        }
    }

    public async Task DeleteAsync(string documentId, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled || string.IsNullOrWhiteSpace(documentId))
        {
            return;
        }

        await EnsureIndexAsync(cancellationToken);
        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            BuildUrl($"{IndexName}/_doc/{Uri.EscapeDataString(documentId)}"));
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Elasticsearch delete failed. DocumentId: {DocumentId}, Status: {Status}, Body: {Body}", documentId, (int)response.StatusCode, Clip(body));
            throw new InvalidOperationException($"Elasticsearch delete failed: {(int)response.StatusCode}.");
        }
    }

    public async Task<SearchHealthResponse> HealthAsync(CancellationToken cancellationToken = default)
    {
        if (!IsEnabled)
        {
            return new SearchHealthResponse
            {
                IsEnabled = false,
                IsAvailable = false,
                IndexName = IndexName,
                Status = "disabled"
            };
        }

        try
        {
            using var response = await _httpClient.GetAsync(BuildUrl("_cluster/health"), cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new SearchHealthResponse
                {
                    IsEnabled = true,
                    IsAvailable = false,
                    IndexName = IndexName,
                    ErrorMessage = Clip(body)
                };
            }

            using var document = JsonDocument.Parse(body);
            var status = document.RootElement.TryGetProperty("status", out var statusElement)
                ? statusElement.GetString()
                : "unknown";

            return new SearchHealthResponse
            {
                IsEnabled = true,
                IsAvailable = true,
                IndexName = IndexName,
                Status = status
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Elasticsearch health check failed.");
            return new SearchHealthResponse
            {
                IsEnabled = true,
                IsAvailable = false,
                IndexName = IndexName,
                ErrorMessage = ex.Message
            };
        }
    }

    private object BuildSearchPayload(SearchRequest request)
    {
        var must = new List<object>();
        var filter = new List<object>();

        if (string.IsNullOrWhiteSpace(request.Query))
        {
            must.Add(new { match_all = new { } });
        }
        else
        {
            must.Add(new
            {
                multi_match = new
                {
                    query = request.Query,
                    fields = new[] { "title^4", "summary^2", "content", "keywords^2" },
                    fuzziness = "AUTO"
                }
            });
        }

        if (request.EntityTypes.Count > 0)
        {
            filter.Add(new { terms = new { entityType = request.EntityTypes } });
        }

        if (request.CustomerId.HasValue)
        {
            filter.Add(new { term = new { customerId = request.CustomerId.Value } });
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            filter.Add(new { term = new { status = request.Status } });
        }

        var amountRange = new Dictionary<string, object>();
        if (request.MinAmount.HasValue) amountRange["gte"] = request.MinAmount.Value;
        if (request.MaxAmount.HasValue) amountRange["lte"] = request.MaxAmount.Value;
        if (amountRange.Count > 0) filter.Add(new { range = new Dictionary<string, object> { ["amount"] = amountRange } });

        var dateRange = new Dictionary<string, object>();
        if (request.From.HasValue) dateRange["gte"] = request.From.Value;
        if (request.To.HasValue) dateRange["lte"] = request.To.Value;
        if (dateRange.Count > 0) filter.Add(new { range = new Dictionary<string, object> { ["createdAt"] = dateRange } });

        return new
        {
            from = (Math.Max(1, request.Page) - 1) * Math.Clamp(request.PageSize, 1, 100),
            size = Math.Clamp(request.PageSize, 1, 100),
            track_total_hits = true,
            query = new
            {
                @bool = new
                {
                    must,
                    filter
                }
            },
            sort = BuildSort(request)
        };
    }

    private static object[] BuildSort(SearchRequest request)
    {
        var direction = request.SortDescending ? "desc" : "asc";
        return request.SortBy.ToLowerInvariant() switch
        {
            "amount" => new object[] { new Dictionary<string, object> { ["amount"] = new { order = direction, missing = "_last" } } },
            "createdat" or "created" => new object[] { new Dictionary<string, object> { ["createdAt"] = new { order = direction, missing = "_last" } } },
            "title" => new object[] { new Dictionary<string, object> { ["title.keyword"] = new { order = direction, missing = "_last" } } },
            _ => new object[] { new Dictionary<string, object> { ["_score"] = new { order = "desc" } } }
        };
    }

    private static SearchResponse ParseSearchResponse(string body, SearchRequest request, TimeSpan took)
    {
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        var hitsRoot = root.GetProperty("hits");
        var total = hitsRoot.GetProperty("total").TryGetProperty("value", out var totalValue)
            ? totalValue.GetInt64()
            : 0;

        var items = new List<SearchHitResponse>();
        foreach (var hit in hitsRoot.GetProperty("hits").EnumerateArray())
        {
            var source = hit.GetProperty("_source").Deserialize<SearchDocument>(SerializerOptions);
            if (source is null) continue;

            items.Add(new SearchHitResponse
            {
                Score = hit.TryGetProperty("_score", out var score) && score.ValueKind == JsonValueKind.Number
                    ? score.GetDouble()
                    : 0,
                Document = source
            });
        }

        return new SearchResponse
        {
            Query = request.Query ?? string.Empty,
            Page = Math.Max(1, request.Page),
            PageSize = Math.Clamp(request.PageSize, 1, 100),
            Total = total,
            IsFromElasticsearch = true,
            Engine = "Elasticsearch",
            Took = took,
            Items = items
        };
    }

    private static bool HasBulkErrors(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return true;
        }

        using var document = JsonDocument.Parse(body);
        return document.RootElement.TryGetProperty("errors", out var errors) && errors.GetBoolean();
    }

    private void ConfigureClient()
    {
        if (!string.IsNullOrWhiteSpace(_configuration.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_configuration.BaseUrl.TrimEnd('/') + "/");
        }

        if (!string.IsNullOrWhiteSpace(_configuration.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", _configuration.ApiKey);
            return;
        }

        if (!string.IsNullOrWhiteSpace(_configuration.Username) &&
            !string.IsNullOrWhiteSpace(_configuration.Password))
        {
            var raw = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration.Username}:{_configuration.Password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", raw);
        }
    }

    private string BuildUrl(string path)
        => path.TrimStart('/');

    private static StringContent ToJsonContent<T>(T payload)
        => new(JsonSerializer.Serialize(payload, SerializerOptions), Encoding.UTF8, "application/json");

    private static string Clip(string value)
        => value.Length <= 800 ? value : value[..800];
}
