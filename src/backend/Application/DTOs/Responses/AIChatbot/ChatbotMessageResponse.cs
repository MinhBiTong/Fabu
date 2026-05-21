namespace Application.DTOs.Responses.AIChatbot;

public sealed class ChatbotMessageResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public bool IsFallback { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
    public List<string> RetrievedSources { get; set; } = new();
    public List<string> SuggestedActions { get; set; } = new();
}
