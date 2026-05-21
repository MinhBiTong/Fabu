using Application.Models.AIChatbot;

namespace Application.Interfaces;

public interface IAiChatCompletionClient
{
    Task<AiChatCompletionResult> CompleteAsync(
        AiChatCompletionRequest request,
        CancellationToken cancellationToken = default);
}
