using Application.DTOs.Requests.AIChatbot;
using Application.DTOs.Responses;
using Application.DTOs.Responses.AIChatbot;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AIChatbotController : ControllerBase
{
    private readonly ICustomerSupportChatbotService _chatbotService;
    private readonly ILogger<AIChatbotController> _logger;

    public AIChatbotController(
        ICustomerSupportChatbotService chatbotService,
        ILogger<AIChatbotController> logger)
    {
        _chatbotService = chatbotService;
        _logger = logger;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromBody] ChatbotMessageRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(ApiResponse<ChatbotMessageResponse>.Fail(400, "Request body không hợp lệ."));
        }

        _logger.LogInformation(
            "Received chatbot request. CustomerId: {CustomerId}, SessionId: {SessionId}",
            request.CustomerId,
            request.SessionId);

        var response = await _chatbotService.SendMessageAsync(request, cancellationToken);
        return ToActionResult(response);
    }

    private IActionResult ToActionResult<T>(ApiResponse<T> response)
    {
        return response.Code switch
        {
            200 => Ok(response),
            400 => BadRequest(response),
            404 => NotFound(response),
            _ => StatusCode(response.Code, response)
        };
    }
}
