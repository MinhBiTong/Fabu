using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Interfaces;
using Application.Models.AIChatbot;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public sealed class ConfigurableAiChatCompletionClient : IAiChatCompletionClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly AIChatbotConfiguration _configuration;
    private readonly ILogger<ConfigurableAiChatCompletionClient> _logger;

    public ConfigurableAiChatCompletionClient(
        HttpClient httpClient,
        IOptions<AIChatbotConfiguration> options,
        ILogger<ConfigurableAiChatCompletionClient> logger)
    {
        _httpClient = httpClient;
        _configuration = options.Value;
        _logger = logger;
    }

    public Task<AiChatCompletionResult> CompleteAsync(
        AiChatCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_configuration.Provider.Equals("Grok", StringComparison.OrdinalIgnoreCase))
        {
            return CompleteWithGrokAsync(request, cancellationToken);
        }

        return CompleteWithGeminiAsync(request, cancellationToken);
    }

    private async Task<AiChatCompletionResult> CompleteWithGeminiAsync(
        AiChatCompletionRequest request,
        CancellationToken cancellationToken)
    {
        var apiKey = ResolveApiKey(_configuration.GeminiApiKey);
        var model = NormalizeGeminiModel(_configuration.GeminiModel);
        var endpoint = $"{TrimTrailingSlash(_configuration.GeminiBaseUrl)}/v1beta/models/{Uri.EscapeDataString(model)}:generateContent";

        var payload = new
        {
            systemInstruction = new
            {
                parts = new[]
                {
                    new { text = request.SystemPrompt }
                }
            },
            contents = request.Messages.Select(message => new
            {
                role = ToGeminiRole(message.Role),
                parts = new[]
                {
                    new { text = message.Content }
                }
            }),
            generationConfig = new
            {
                temperature = request.Temperature,
                maxOutputTokens = request.MaxOutputTokens
            }
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
        httpRequest.Headers.Add("x-goog-api-key", apiKey);
        httpRequest.Content = BuildJsonContent(payload);

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Gemini API returned {StatusCode}. Body: {Body}",
                (int)response.StatusCode,
                Clip(body));
            throw new InvalidOperationException($"Gemini API returned {(int)response.StatusCode}.");
        }

        return ParseGeminiResponse(body, model);
    }

    private async Task<AiChatCompletionResult> CompleteWithGrokAsync(
        AiChatCompletionRequest request,
        CancellationToken cancellationToken)
    {
        var apiKey = ResolveApiKey(_configuration.GrokApiKey);
        var model = _configuration.GrokModel;
        var endpoint = $"{TrimTrailingSlash(_configuration.GrokBaseUrl)}/chat/completions";

        var messages = new List<object>
        {
            new { role = "system", content = request.SystemPrompt }
        };
        messages.AddRange(request.Messages.Select(message => new
        {
            role = ToOpenAiRole(message.Role),
            content = message.Content
        }));

        var payload = new
        {
            model,
            messages,
            temperature = request.Temperature,
            max_tokens = request.MaxOutputTokens
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        httpRequest.Content = BuildJsonContent(payload);

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Grok API returned {StatusCode}. Body: {Body}",
                (int)response.StatusCode,
                Clip(body));
            throw new InvalidOperationException($"Grok API returned {(int)response.StatusCode}.");
        }

        return ParseGrokResponse(body, model);
    }

    private AiChatCompletionResult ParseGeminiResponse(string body, string model)
    {
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        var textBuilder = new StringBuilder();

        if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
        {
            var candidate = candidates[0];
            if (candidate.TryGetProperty("content", out var content) &&
                content.TryGetProperty("parts", out var parts))
            {
                foreach (var part in parts.EnumerateArray())
                {
                    if (part.TryGetProperty("text", out var textElement))
                    {
                        textBuilder.Append(textElement.GetString());
                    }
                }
            }
        }

        var answer = textBuilder.ToString().Trim();
        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new InvalidOperationException("Gemini API returned an empty answer.");
        }

        var promptTokens = TryReadInt(root, "usageMetadata", "promptTokenCount");
        var completionTokens = TryReadInt(root, "usageMetadata", "candidatesTokenCount");

        return new AiChatCompletionResult(
            answer,
            "Gemini",
            model,
            promptTokens,
            completionTokens,
            TryReadFinishReason(root));
    }

    private static AiChatCompletionResult ParseGrokResponse(string body, string model)
    {
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        var answer = root
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new InvalidOperationException("Grok API returned an empty answer.");
        }

        var promptTokens = TryReadInt(root, "usage", "prompt_tokens");
        var completionTokens = TryReadInt(root, "usage", "completion_tokens");
        var finishReason = root.GetProperty("choices")[0].TryGetProperty("finish_reason", out var finish)
            ? finish.GetString()
            : null;

        return new AiChatCompletionResult(
            answer.Trim(),
            "Grok",
            model,
            promptTokens,
            completionTokens,
            finishReason);
    }

    private string ResolveApiKey(string providerSpecificApiKey)
    {
        var apiKey = !string.IsNullOrWhiteSpace(providerSpecificApiKey)
            ? providerSpecificApiKey
            : _configuration.ApiKey;

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("AI chatbot API key is not configured.");
        }

        return apiKey;
    }

    private static string NormalizeGeminiModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
        {
            return "gemini-1.5-flash";
        }

        return model.StartsWith("models/", StringComparison.OrdinalIgnoreCase)
            ? model["models/".Length..]
            : model;
    }

    private static string ToGeminiRole(string role)
        => role.Equals("assistant", StringComparison.OrdinalIgnoreCase) ? "model" : "user";

    private static string ToOpenAiRole(string role)
        => role.Equals("assistant", StringComparison.OrdinalIgnoreCase) ? "assistant" : "user";

    private static StringContent BuildJsonContent<T>(T payload)
        => new(JsonSerializer.Serialize(payload, SerializerOptions), Encoding.UTF8, "application/json");

    private static string TrimTrailingSlash(string value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.TrimEnd('/');

    private static int? TryReadInt(JsonElement root, string parentName, string propertyName)
    {
        if (root.TryGetProperty(parentName, out var parent) &&
            parent.TryGetProperty(propertyName, out var value) &&
            value.TryGetInt32(out var result))
        {
            return result;
        }

        return null;
    }

    private static string? TryReadFinishReason(JsonElement root)
    {
        if (root.TryGetProperty("candidates", out var candidates) &&
            candidates.GetArrayLength() > 0 &&
            candidates[0].TryGetProperty("finishReason", out var finishReason))
        {
            return finishReason.GetString();
        }

        return null;
    }

    private static string Clip(string value)
        => value.Length <= 500 ? value : value[..500];
}
