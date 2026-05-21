namespace Application.Models.AIChatbot;

public sealed record AiChatCompletionRequest(
    string SystemPrompt,
    IReadOnlyList<AiChatMessage> Messages,
    int MaxOutputTokens,
    double Temperature);

public sealed record AiChatCompletionResult(
    string Content,
    string Provider,
    string Model,
    int? PromptTokens = null,
    int? CompletionTokens = null,
    string? FinishReason = null);

public sealed record AiChatMessage(string Role, string Content);
