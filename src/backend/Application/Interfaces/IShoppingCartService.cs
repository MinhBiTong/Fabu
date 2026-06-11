using Application.DTOs.Requests.CartRequest;
using Application.DTOs.Responses.CartResponse;
using Application.DTOs.Responses.OrderResponse;
using Application.DTOs.Responses.PaymentResponse;

namespace Application.Interfaces
{
    public interface IShoppingCartService
    {
        Task<CartResponse> GetActiveCartAsync(long customerId);
        Task<CartResponse> AddItemAsync(CartItemRequest request);
        Task<CartResponse> UpdateItemAsync(CartItemRequest request);
        Task<CartResponse> RemoveItemAsync(long customerId, long productId);
        Task<OrderCheckoutResponse> CheckoutAsync(CartCheckoutRequest request);
    }

    public class OrderCheckoutResponse
    {
        public OrderResponse Order { get; set; } = new();
        public PaymentResponse Payment { get; set; } = new();
    }
}
