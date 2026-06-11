using Application.DTOs.Requests.CartRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.CartResponse;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IShoppingCartService _cartService;

        public CartController(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("customer/{customerId:long}")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> GetActiveCart(long customerId)
        {
            var cart = await _cartService.GetActiveCartAsync(customerId);
            return Ok(ApiResponse<CartResponse>.Success(cart));
        }

        [HttpPost("items")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> AddItem([FromBody] CartItemRequest request)
        {
            var cart = await _cartService.AddItemAsync(request);
            return Ok(ApiResponse<CartResponse>.Success(cart, "Cart item added successfully."));
        }

        [HttpPut("items")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> UpdateItem([FromBody] CartItemRequest request)
        {
            var cart = await _cartService.UpdateItemAsync(request);
            return Ok(ApiResponse<CartResponse>.Success(cart, "Cart item updated successfully."));
        }

        [HttpDelete("customer/{customerId:long}/items/{productId:long}")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> RemoveItem(long customerId, long productId)
        {
            var cart = await _cartService.RemoveItemAsync(customerId, productId);
            return Ok(ApiResponse<CartResponse>.Success(cart, "Cart item removed successfully."));
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<ApiResponse<OrderCheckoutResponse>>> Checkout([FromBody] CartCheckoutRequest request)
        {
            try
            {
                var checkout = await _cartService.CheckoutAsync(request);
                return Ok(ApiResponse<OrderCheckoutResponse>.Success(checkout, "Checkout created successfully."));
            }
            catch (AppException ex)
            {
                return BadRequest(ApiResponse<OrderCheckoutResponse>.Fail((int)ex.GetErrorCode(), ex.Message));
            }
        }
    }
}
