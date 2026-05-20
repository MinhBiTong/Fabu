using Application.DTOs.Requests.RechargePlanRequest;
using Application.DTOs.Requests.TransactionRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.TransactionResponse;
using Application.Features.Transactions.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Exceptions;
using MediatR;
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
        private readonly IMediator _mediator;

        public TransactionController(ITransactionService transactionService, IMediator mediator)
        {
            _transactionService = transactionService;
            _mediator = mediator;
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
            var result = await _mediator.Send(new GetTransactionsByCustomerQuery(customerId, page, pageSize));
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin một giao dịch theo TransactionRef
        /// </summary>
        [HttpGet("{transactionRef}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<TransactionResponse>>> GetTransactionByRef(string transactionRef)
        {
            var result = await _mediator.Send(new GetTransactionByRefQuery(transactionRef));
            return result.Code == 200 ? Ok(result) : NotFound(result);
        }
    }
}
