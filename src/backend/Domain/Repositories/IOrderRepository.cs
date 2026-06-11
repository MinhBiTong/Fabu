using Domain.Abstractions.Repositories;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IOrderRepository : IRepositoryBase<Order, Guid>
    {
        Task<Order?> GetByOrderCodeAsync(string orderCode);
        Task<Order?> GetOrderWithItemsAsync(Guid orderId);
        Task<List<Order>> GetOrdersByCustomerAsync(long customerId, int skip, int take);
        Task<int> CountOrdersByCustomerAsync(long customerId);
    }
}
