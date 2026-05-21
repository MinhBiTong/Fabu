using System.Security.Cryptography;
using Application.DTOs.Requests.SmsRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.SmsResponse;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SendSMSController : ControllerBase
    {
        private readonly ISmsService _sms;
        private readonly ILogger<SendSMSController> _logger;

        public SendSMSController(ISmsService sms, ILogger<SendSMSController> logger)
        {
            _sms = sms;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Send(
            [FromBody] SmsSendRequest? request,
            CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return BadRequest(ApiResponse<SmsSendResult>.Fail(400, "Request body khong hop le."));
            }

            var result = await _sms.SendSmsAsync(request.Phone, request.Message, cancellationToken);
            return ToActionResult(result);
        }

        [HttpPost("otp")]
        public async Task<IActionResult> SendOtp(
            [FromBody] SmsOtpRequest? request,
            CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return BadRequest(ApiResponse<SmsSendResult>.Fail(400, "Request body khong hop le."));
            }

            var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var result = await _sms.SendOtpAsync(request.Phone, otp, cancellationToken);

            _logger.LogInformation(
                "OTP SMS requested. Phone: {Phone}, IsSuccess: {IsSuccess}",
                MaskPhone(result.Phone),
                result.IsSuccess);

            return ToActionResult(result);
        }

        private static IActionResult ToActionResult(SmsSendResult result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(ApiResponse<SmsSendResult>.Success(result, "SMS sent successfully."));
            }

            var code = result.HttpStatusCode.HasValue && result.HttpStatusCode.Value >= 500 ? 502 : 400;
            return new ObjectResult(ApiResponse<SmsSendResult>.Fail(code, result.ErrorMessage ?? "SMS send failed."))
            {
                StatusCode = code
            };
        }

        private static string MaskPhone(string phone)
            => string.IsNullOrWhiteSpace(phone) || phone.Length <= 4
                ? "****"
                : $"{new string('*', Math.Max(0, phone.Length - 4))}{phone[^4..]}";
    }
}
