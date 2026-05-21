using Application.Models.AIChatbot;

namespace Application.Interfaces;

public interface IChatbotConversationStore
{
    Task<IReadOnlyList<ChatbotConversationMessage>> GetHistoryAsync(
        string sessionId,
        CancellationToken cancellationToken = default);

    Task SaveTurnAsync(
        string sessionId,
        string userMessage,
        string assistantMessage,
        int maxMessages,
        TimeSpan ttl,
        CancellationToken cancellationToken = default);

    Task ClearAsync(string sessionId, CancellationToken cancellationToken = default);
}
