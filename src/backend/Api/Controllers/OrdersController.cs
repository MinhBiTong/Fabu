using Application.DTOs.Responses;
using Application.DTOs.Responses.OrderResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> GetById(Guid orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            return Ok(ApiResponse<OrderResponse>.Success(order));
        }

        [HttpGet("code/{orderCode}")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> GetByCode(string orderCode)
        {
            var order = await _orderService.GetByCodeAsync(orderCode);
            return Ok(ApiResponse<OrderResponse>.Success(order));
        }

        [HttpGet("customer/{customerId:long}")]
        public async Task<ActionResult<ApiResponse<PagedResult<OrderResponse>>>> GetByCustomer(
            long customerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetOrdersByCustomerAsync(customerId, page, pageSize);
            return Ok(ApiResponse<PagedResult<OrderResponse>>.Success(orders));
        }

        [HttpPost("{orderId:guid}/cancel")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> Cancel(Guid orderId)
        {
            try
            {
                var order = await _orderService.CancelAsync(orderId);
                return Ok(ApiResponse<OrderResponse>.Success(order, "Order cancelled successfully."));
            }
            catch (AppException ex)
            {
                return BadRequest(ApiResponse<OrderResponse>.Fail((int)ex.GetErrorCode(), ex.Message));
            }
        }
    }
}
