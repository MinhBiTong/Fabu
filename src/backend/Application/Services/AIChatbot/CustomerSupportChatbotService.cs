using Application.DTOs.Requests.AIChatbot;
using Application.DTOs.Responses;
using Application.DTOs.Responses.AIChatbot;
using Application.Interfaces;
using Application.Models.AIChatbot;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.AIChatbot;

public sealed class CustomerSupportChatbotService : ICustomerSupportChatbotService
{
    private readonly ICustomerSupportRagService _ragService;
    private readonly ICustomerSupportPromptBuilder _promptBuilder;
    private readonly IAiChatCompletionClient _chatCompletionClient;
    private readonly IChatbotConversationStore _conversationStore;
    private readonly AIChatbotConfiguration _configuration;
    private readonly ILogger<CustomerSupportChatbotService> _logger;

    public CustomerSupportChatbotService(
        ICustomerSupportRagService ragService,
        ICustomerSupportPromptBuilder promptBuilder,
        IAiChatCompletionClient chatCompletionClient,
        IChatbotConversationStore conversationStore,
        IOptions<AIChatbotConfiguration> options,
        ILogger<CustomerSupportChatbotService> logger)
    {
        _ragService = ragService;
        _promptBuilder = promptBuilder;
        _chatCompletionClient = chatCompletionClient;
        _conversationStore = conversationStore;
        _configuration = options.Value;
        _logger = logger;
    }

    public async Task<ApiResponse<ChatbotMessageResponse>> SendMessageAsync(
        ChatbotMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Message))
        {
            return ApiResponse<ChatbotMessageResponse>.Fail(400, "Tin nhắn không được để trống.");
        }

        if (request.Message.Length > 1000)
        {
            return ApiResponse<ChatbotMessageResponse>.Fail(400, "Tin nhắn tối đa 1000 ký tự.");
        }

        var sessionId = string.IsNullOrWhiteSpace(request.SessionId)
            ? Guid.NewGuid().ToString("N")
            : request.SessionId.Trim();

        try
        {
            if (request.ResetContext)
            {
                await _conversationStore.ClearAsync(sessionId, cancellationToken);
            }

            var ragContext = await _ragService.BuildContextAsync(
                request.CustomerId,
                request.Message,
                cancellationToken);

            var history = await _conversationStore.GetHistoryAsync(sessionId, cancellationToken);
            var systemPrompt = _promptBuilder.BuildSystemPrompt();
            var messages = _promptBuilder.BuildMessages(request.Message, ragContext, history);

            var aiResult = await CompleteWithFallbackAsync(
                systemPrompt,
                messages,
                request.Message,
                ragContext,
                cancellationToken);

            await _conversationStore.SaveTurnAsync(
                sessionId,
                request.Message,
                aiResult.Content,
                Math.Max(2, _configuration.MaxHistoryMessages),
                TimeSpan.FromMinutes(Math.Max(5, _configuration.MemoryTtlMinutes)),
                cancellationToken);

            var response = new ChatbotMessageResponse
            {
                SessionId = sessionId,
                Answer = aiResult.Content,
                Provider = aiResult.Provider,
                Model = aiResult.Model,
                IsFallback = aiResult.Provider.Equals("Fallback", StringComparison.OrdinalIgnoreCase),
                GeneratedAt = DateTimeOffset.UtcNow,
                RetrievedSources = ragContext.RetrievedSources.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                SuggestedActions = _promptBuilder.BuildSuggestedActions(request.Message, ragContext).ToList()
            };

            return ApiResponse<ChatbotMessageResponse>.Success(response, "Chatbot trả lời thành công.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Chatbot request was cancelled. SessionId: {SessionId}", sessionId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chatbot request failed. SessionId: {SessionId}, CustomerId: {CustomerId}", sessionId, request.CustomerId);
            return ApiResponse<ChatbotMessageResponse>.Fail(500, "Chatbot đang gặp lỗi, vui lòng thử lại sau.");
        }
    }

    private async Task<AiChatCompletionResult> CompleteWithFallbackAsync(
        string systemPrompt,
        IReadOnlyList<AiChatMessage> messages,
        string userMessage,
        ChatbotRagContext ragContext,
        CancellationToken cancellationToken)
    {
        if (!_configuration.Enabled || string.IsNullOrWhiteSpace(_configuration.ResolveApiKey()))
        {
            _logger.LogWarning(
                "AI chatbot provider is disabled or missing API key. Provider: {Provider}",
                _configuration.Provider);

            if (!_configuration.UseFallbackWhenDisabled)
            {
                throw new InvalidOperationException("AI chatbot provider is not configured.");
            }

            return BuildFallbackResult(userMessage, ragContext);
        }

        try
        {
            return await _chatCompletionClient.CompleteAsync(
                new AiChatCompletionRequest(
                    systemPrompt,
                    messages,
                    Math.Max(128, _configuration.MaxOutputTokens),
                    Math.Clamp(_configuration.Temperature, 0, 1)),
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI provider call failed. Provider: {Provider}", _configuration.Provider);

            if (!_configuration.UseFallbackWhenDisabled)
            {
                throw;
            }

            return BuildFallbackResult(userMessage, ragContext);
        }
    }

    private AiChatCompletionResult BuildFallbackResult(string userMessage, ChatbotRagContext ragContext)
    {
        var normalized = userMessage.ToLowerInvariant();
        var answer = new List<string>
        {
            "Mình có thể hỗ trợ dựa trên dữ liệu hiện có của Fabu."
        };

        if (normalized.Contains("số dư") || normalized.Contains("balance"))
        {
            answer.Add(ragContext.AccountSummary is null
                ? "Mình chưa có dữ liệu tài khoản. Bạn vui lòng đăng nhập hoặc gửi CustomerId để kiểm tra số dư."
                : $"Tài khoản hiện tại: {ragContext.AccountSummary}.");
        }
        else if (normalized.Contains("giao dịch") || normalized.Contains("lịch sử"))
        {
            answer.Add(ragContext.RecentTransactions.Count == 0
                ? "Mình chưa thấy giao dịch gần đây trong dữ liệu truy xuất."
                : "Giao dịch gần đây:\n- " + string.Join("\n- ", ragContext.RecentTransactions));
        }
        else if (normalized.Contains("hóa đơn") || normalized.Contains("hoá đơn"))
        {
            answer.Add(ragContext.UnpaidBills.Count == 0
                ? "Hiện chưa có hóa đơn trả sau chưa tất toán trong dữ liệu truy xuất."
                : "Hóa đơn cần chú ý:\n- " + string.Join("\n- ", ragContext.UnpaidBills));
        }
        else if (normalized.Contains("khiếu nại") || normalized.Contains("lỗi"))
        {
            answer.Add("Để tạo khiếu nại, bạn nên cung cấp mã giao dịch/hóa đơn, số tiền, thời gian phát sinh và mô tả lỗi để admin kiểm tra.");
        }
        else if (normalized.Contains("thanh toán"))
        {
            answer.Add("Bạn có thể chọn gói hoặc hóa đơn, kiểm tra số tiền, chọn VNPay/PayPal/Stripe hoặc số dư tài khoản, xác nhận thanh toán và lưu mã giao dịch.");
        }
        else
        {
            answer.Add(ragContext.ActiveRechargePlans.Count == 0
                ? "Hiện chưa có gói nạp đang hoạt động trong dữ liệu truy xuất."
                : "Một số gói nạp đang hoạt động:\n- " + string.Join("\n- ", ragContext.ActiveRechargePlans.Take(3)));
        }

        return new AiChatCompletionResult(
            string.Join(Environment.NewLine, answer),
            "Fallback",
            "RuleBased");
    }
}
