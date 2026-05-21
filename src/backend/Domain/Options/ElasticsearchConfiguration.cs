namespace Domain.Options;

public sealed class ElasticsearchConfiguration
{
    public bool Enabled { get; set; } = false;
    public string BaseUrl { get; set; } = "http://localhost:9200";
    public string IndexName { get; set; } = "fabu-global-v1";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 10;
    public bool SyncOnSave { get; set; } = true;
    public bool UseDatabaseFallback { get; set; } = true;
    public int MaxBulkSize { get; set; } = 500;
    public int NumberOfShards { get; set; } = 1;
    public int NumberOfReplicas { get; set; } = 0;
}
