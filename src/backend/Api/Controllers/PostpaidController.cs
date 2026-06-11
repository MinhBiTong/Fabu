using Application.DTOs.Requests.PostpaidRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.PaymentResponse;
using Application.DTOs.Responses.PostpaidResponse;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class PostpaidController : ControllerBase
    {
        private readonly IPostpaidBillService _postpaidBillService;

        public PostpaidController(IPostpaidBillService postpaidBillService)
        {
            _postpaidBillService = postpaidBillService;
        }

        [HttpPost("bills")]
        public async Task<ActionResult<ApiResponse<PostpaidBillResponse>>> CreateBill([FromBody] PostpaidCreateRequest request)
        {
            var result = await _postpaidBillService.CreateAsync(request);
            return Ok(ApiResponse<PostpaidBillResponse>.Success(result, "Postpaid bill created successfully."));
        }

        [HttpGet("customer/{customerId:long}/unpaid")]
        public async Task<ActionResult<ApiResponse<List<PostpaidBillResponse>>>> GetUnpaidBills(long customerId)
        {
            var result = await _postpaidBillService.GetUnpaidBillsByCustomerAsync(customerId);
            return Ok(ApiResponse<List<PostpaidBillResponse>>.Success(result));
        }

        [HttpGet("customer/{customerId:long}/latest")]
        public async Task<ActionResult<ApiResponse<PostpaidBillResponse>>> GetLatestBill(long customerId)
        {
            var result = await _postpaidBillService.GetLatestBillAsync(customerId);
            return Ok(ApiResponse<PostpaidBillResponse>.Success(result));
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<ApiResponse<List<PostpaidBillResponse>>>> GetOverdueBills()
        {
            var result = await _postpaidBillService.GetOverdueBillsAsync();
            return Ok(ApiResponse<List<PostpaidBillResponse>>.Success(result));
        }

        [HttpPost("bills/{billId:long}/pay")]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> PayBill(long billId, [FromBody] PostpaidPaymentRequest request)
        {
            try
            {
                var result = await _postpaidBillService.PayBillAsync(billId, request);
                return Ok(ApiResponse<PaymentResponse>.Success(result, "Postpaid bill payment created successfully."));
            }
            catch (AppException ex)
            {
                return BadRequest(ApiResponse<PaymentResponse>.Fail((int)ex.GetErrorCode(), ex.Message));
            }
        }
    }
}
