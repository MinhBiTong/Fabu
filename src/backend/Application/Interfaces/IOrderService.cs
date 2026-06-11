using Application.DTOs.Responses.OrderResponse;
using Domain.Abstractions;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> GetByCodeAsync(string orderCode);
        Task<OrderResponse> GetByIdAsync(Guid orderId);
        Task<PagedResult<OrderResponse>> GetOrdersByCustomerAsync(long customerId, int page = 1, int pageSize = 10);
        Task<OrderResponse> CancelAsync(Guid orderId);
    }
}
