using System.Text.Json;
using Application.Interfaces;
using Application.Models.AIChatbot;
using Microsoft.Extensions.Logging;

namespace Application.Services.AIChatbot;

public sealed class ChatbotConversationStore : IChatbotConversationStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IResponseCacheService _cacheService;
    private readonly ILogger<ChatbotConversationStore> _logger;

    public ChatbotConversationStore(
        IResponseCacheService cacheService,
        ILogger<ChatbotConversationStore> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ChatbotConversationMessage>> GetHistoryAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Array.Empty<ChatbotConversationMessage>();
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var raw = await _cacheService.GetRawStringAsync(BuildKey(sessionId));
            if (string.IsNullOrWhiteSpace(raw))
            {
                return Array.Empty<ChatbotConversationMessage>();
            }

            return JsonSerializer.Deserialize<List<ChatbotConversationMessage>>(raw, SerializerOptions)
                ?? new List<ChatbotConversationMessage>();
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Chatbot history JSON is invalid for session {SessionId}", sessionId);
            return Array.Empty<ChatbotConversationMessage>();
        }
    }

    public async Task SaveTurnAsync(
        string sessionId,
        string userMessage,
        string assistantMessage,
        int maxMessages,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var history = (await GetHistoryAsync(sessionId, cancellationToken)).ToList();
        history.Add(new ChatbotConversationMessage
        {
            Role = "user",
            Content = userMessage,
            CreatedAt = DateTimeOffset.UtcNow
        });
        history.Add(new ChatbotConversationMessage
        {
            Role = "assistant",
            Content = assistantMessage,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var boundedHistory = history
            .Where(message => !string.IsNullOrWhiteSpace(message.Content))
            .TakeLast(Math.Max(2, maxMessages))
            .ToList();

        var json = JsonSerializer.Serialize(boundedHistory, SerializerOptions);
        await _cacheService.SetRawStringAsync(BuildKey(sessionId), json, ttl);
    }

    public Task ClearAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Task.CompletedTask;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return _cacheService.RemoveCacheResponseAsync(BuildKey(sessionId));
    }

    private static string BuildKey(string sessionId)
        => $"v1:chatbot:session:{sessionId.Trim()}";
}
