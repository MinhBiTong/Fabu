using Application.Models.AIChatbot;

namespace Application.Interfaces;

public interface ICustomerSupportPromptBuilder
{
    string BuildSystemPrompt();

    IReadOnlyList<AiChatMessage> BuildMessages(
        string userMessage,
        ChatbotRagContext ragContext,
        IReadOnlyList<ChatbotConversationMessage> history);

    IReadOnlyList<string> BuildSuggestedActions(string userMessage, ChatbotRagContext ragContext);
}
