using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.PaymentResponse;
using Application.Features.Payments.Queries;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
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
        private readonly IMediator _mediator;
        public PaymentController(IPaymentService paymentService, IMediator mediator)
        {
            _paymentService = paymentService;
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo giao dịch thanh toán và trả về URL chuyển hướng đến gateway nếu cần
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
            catch (AppException)
            {
                return BadRequest(ApiResponse<PaymentResponse>.Fail(400, "Create payment request failed"));
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
            var result = await _paymentService.HandleVNPayCallbackAsync(callbackData);
            return BuildGatewayRedirect(result);
        }

        [HttpGet("paypal-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> PayPalCallback([FromQuery] Dictionary<string, string> callbackData)
        {
            var result = await _paymentService.HandlePaymentCallbackAsync("PayPal", callbackData);
            return BuildGatewayRedirect(result);
        }

        [HttpGet("stripe-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeCallback([FromQuery] Dictionary<string, string> callbackData)
        {
            var result = await _paymentService.HandlePaymentCallbackAsync("Stripe", callbackData);
            return BuildGatewayRedirect(result);
        }

        [HttpGet("paypal-cancel")]
        [AllowAnonymous]
        public IActionResult PayPalCancel([FromQuery] string? paymentRef)
            => Redirect($"https://your-frontend.com/payment-failed?ref={Uri.EscapeDataString(paymentRef ?? string.Empty)}&message=PayPal+payment+cancelled");

        [HttpGet("stripe-cancel")]
        [AllowAnonymous]
        public IActionResult StripeCancel([FromQuery] string? paymentRef)
            => Redirect($"https://your-frontend.com/payment-failed?ref={Uri.EscapeDataString(paymentRef ?? string.Empty)}&message=Stripe+payment+cancelled");

        /// <summary>
        /// Lấy thông tin một giao dịch thanh toán theo PaymentRef
        /// </summary>
        [HttpGet("{paymentRef}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetPayment(string paymentRef)
        {
            var response = await _mediator.Send(new GetPaymentByRefQuery(paymentRef));
            return response.Code == 200 ? Ok(response) : NotFound(response);
        }

        private IActionResult BuildGatewayRedirect(Domain.Options.PaymentCallbackResult result)
        {
            if (result.IsSuccess)
            {
                return Redirect($"https://your-frontend.com/payment-success?ref={Uri.EscapeDataString(result.PaymentRef)}");
            }

            return Redirect($"https://your-frontend.com/payment-failed?message={Uri.EscapeDataString(result.Message)}");
        }
    }
}
