using Application.DTOs.Requests.RechargePlanRequest;
using Application.DTOs.Requests.TransactionRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.TransactionResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize] //global auth
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Tạo giao dịch nạp tiền (Recharge)
        /// </summary>
        [HttpPost("recharge")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<TransactionResponse>>> CreateRecharge([FromBody] TransactionCreateRequest request)
        {
            try
            {
                var result = await _transactionService.CreateRechargeTransactionAsync(request);
                return Ok(ApiResponse<TransactionResponse>.Success(result, "Create recharge transaction request successfully"));
            }
            catch (AppException ex)
            {
                return BadRequest(ApiResponse<TransactionResponse>.Fail(500, "Error the system create recharge transaction request"));
            }
        }

        /// <summary>
        /// Lấy danh sách giao dịch của khách hàng
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<TransactionResponse>>>> GetTransactionsByCustomer(
            long customerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _transactionService.GetTransactionsByCustomerAsync(customerId, page, pageSize);
            return Ok(ApiResponse<PagedResult<TransactionResponse>>.Success(result));
        }

        /// <summary>
        /// Lấy thông tin một giao dịch theo TransactionRef
        /// </summary>
        [HttpGet("{transactionRef}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<TransactionResponse>>> GetTransactionByRef(string transactionRef)
        {
            try
            {
                var result = await _transactionService.GetTransactionByRefAsync(transactionRef);
                return Ok(ApiResponse<TransactionResponse>.Success(result));
            }
            catch (AppException ex)
            {
                return NotFound(ApiResponse<TransactionResponse>.Fail(500, "Error the system not found transaction by ref"));
            }
        }
    }
}
