using Application.DTOs.Responses.OrderResponse;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponse> CancelAsync(Guid orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            if (order is null)
                throw new AppException(ErrorCode.INVALID_KEY, "Order not found.");

            if (order.Status is OrderStatus.Paid or OrderStatus.Processing or OrderStatus.Completed)
                throw new AppException(ErrorCode.INVALID_KEY, "Paid or fulfilled orders cannot be cancelled here.");

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTimeOffset.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            return OrderResponse.FromEntity(order);
        }

        public async Task<OrderResponse> GetByCodeAsync(string orderCode)
        {
            var order = await _unitOfWork.Orders.GetByOrderCodeAsync(orderCode);
            if (order is null)
                throw new AppException(ErrorCode.INVALID_KEY, "Order not found.");

            return OrderResponse.FromEntity(order);
        }

        public async Task<OrderResponse> GetByIdAsync(Guid orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            if (order is null)
                throw new AppException(ErrorCode.INVALID_KEY, "Order not found.");

            return OrderResponse.FromEntity(order);
        }

        public async Task<PagedResult<OrderResponse>> GetOrdersByCustomerAsync(long customerId, int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var total = await _unitOfWork.Orders.CountOrdersByCustomerAsync(customerId);
            var orders = await _unitOfWork.Orders.GetOrdersByCustomerAsync(customerId, (page - 1) * pageSize, pageSize);

            return new PagedResult<OrderResponse>(
                orders.Select(OrderResponse.FromEntity).ToList(),
                total,
                pageSize);
        }
    }
}
