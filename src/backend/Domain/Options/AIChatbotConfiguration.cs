namespace Domain.Options;

public sealed class AIChatbotConfiguration
{
    public bool Enabled { get; set; } = true;
    public string Provider { get; set; } = "Gemini";

    public string ApiKey { get; set; } = string.Empty;
    public string GeminiApiKey { get; set; } = string.Empty;
    public string GeminiBaseUrl { get; set; } = "https://generativelanguage.googleapis.com";
    public string GeminiModel { get; set; } = "gemini-1.5-flash";

    public string GrokApiKey { get; set; } = string.Empty;
    public string GrokBaseUrl { get; set; } = "https://api.x.ai/v1";
    public string GrokModel { get; set; } = "grok-4.3";

    public int MaxHistoryMessages { get; set; } = 8;
    public int MemoryTtlMinutes { get; set; } = 60;
    public int MaxRecentTransactions { get; set; } = 5;
    public int MaxActivePlans { get; set; } = 10;
    public int MaxOutputTokens { get; set; } = 700;
    public double Temperature { get; set; } = 0.2;
    public bool UseFallbackWhenDisabled { get; set; } = true;

    public string ResolveModel()
        => Provider.Equals("Grok", StringComparison.OrdinalIgnoreCase)
            ? GrokModel
            : GeminiModel;

    public string ResolveApiKey()
    {
        if (!string.IsNullOrWhiteSpace(ApiKey))
        {
            return ApiKey;
        }

        return Provider.Equals("Grok", StringComparison.OrdinalIgnoreCase)
            ? GrokApiKey
            : GeminiApiKey;
    }
}
