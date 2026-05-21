using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.AIChatbot;

public sealed class ChatbotMessageRequest
{
    public long? CustomerId { get; set; }

    public string? SessionId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Message { get; set; } = string.Empty;

    public bool ResetContext { get; set; }
}
