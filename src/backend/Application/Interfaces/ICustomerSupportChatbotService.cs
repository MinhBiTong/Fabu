using Application.DTOs.Requests.AIChatbot;
using Application.DTOs.Responses;
using Application.DTOs.Responses.AIChatbot;

namespace Application.Interfaces;

public interface ICustomerSupportChatbotService
{
    Task<ApiResponse<ChatbotMessageResponse>> SendMessageAsync(
        ChatbotMessageRequest request,
        CancellationToken cancellationToken = default);
}
