using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.PaymentResponse;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize] //global auth
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Tạo giao dịch thanh toán và trả về URL chuyển hướng đến VNPay
        /// </summary>
        [HttpPost]
        [Authorize]   // Hoặc [AllowAnonymous] nếu cho phép guest thanh toán
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> CreatePayment([FromBody] PaymentCreateRequest request)
        {
            if (request == null || request.Amount <= 0)
                return BadRequest(ApiResponse<PaymentResponse>.Fail(400, "Invalid payment data"));

            try
            {
                var result = await _paymentService.CreatePaymentAsync(request);
                return Ok(ApiResponse<PaymentResponse>.Success(result, "Create payment request successfully"));
            }
            catch (AppException ex)
            {
                return BadRequest(ApiResponse<PaymentResponse>.Fail(200, "Create payment request failed"));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<PaymentResponse>.Fail(500, "Error system payment"));
            }
        }

        /// <summary>
        /// Callback từ VNPay sau khi thanh toán (VNPay sẽ gọi endpoint này)
        /// </summary>
        [HttpGet("vnpay-callback")]
        [AllowAnonymous]   // VNPay gọi từ ngoài, không cần auth
        public async Task<IActionResult> VNPayCallback([FromQuery] Dictionary<string, string> callbackData)
        {
            try
            {
                var result = await _paymentService.HandleVNPayCallbackAsync(callbackData);

                if (result.IsSuccess)
                {
                    // Redirect về trang thành công trên frontend
                    return Redirect($"https://your-frontend.com/payment-success?ref={result.TransactionRef}");
                }
                else
                {
                    return Redirect($"https://your-frontend.com/payment-failed?message={Uri.EscapeDataString(result.Message)}");
                }
            }
            catch (Exception ex)
            {
                return Redirect("https://your-frontend.com/payment-failed?message=System+error");
            }
        }

        /// <summary>
        /// Lấy thông tin một giao dịch thanh toán theo PaymentRef
        /// </summary>
        [HttpGet("{paymentRef}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetPayment(string paymentRef)
        {
            try
            {
                var result = await _paymentService.GetPaymentByRefAsync(paymentRef);
                return Ok(ApiResponse<PaymentResponse>.Success(result));
            }
            catch (AppException ex)
            {
                return NotFound(ApiResponse<PaymentResponse>.Fail(500, "Not found payment by Ref "));
            }
        }
    }
}
