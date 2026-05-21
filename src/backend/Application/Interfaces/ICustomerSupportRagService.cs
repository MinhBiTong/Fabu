using Application.Models.AIChatbot;

namespace Application.Interfaces;

public interface ICustomerSupportRagService
{
    Task<ChatbotRagContext> BuildContextAsync(
        long? customerId,
        string message,
        CancellationToken cancellationToken = default);
}
