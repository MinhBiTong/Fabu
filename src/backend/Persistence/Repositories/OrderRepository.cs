using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;

namespace Persistence.Repositories
{
    public class OrderRepository : BaseRepository<Order, Guid>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<int> CountOrdersByCustomerAsync(long customerId)
        {
            return await _dbSet.CountAsync(order => order.CustomerId == customerId && !order.IsDeleted);
        }

        public async Task<Order?> GetByOrderCodeAsync(string orderCode)
        {
            return await _dbSet
                .Include(order => order.Items)
                .Include(order => order.Payment)
                .FirstOrDefaultAsync(order => order.OrderCode == orderCode && !order.IsDeleted);
        }

        public async Task<List<Order>> GetOrdersByCustomerAsync(long customerId, int skip, int take)
        {
            return await _dbSet
                .Include(order => order.Items)
                .Include(order => order.Payment)
                .Where(order => order.CustomerId == customerId && !order.IsDeleted)
                .OrderByDescending(order => order.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithItemsAsync(Guid orderId)
        {
            return await _dbSet
                .Include(order => order.Customer)
                .Include(order => order.Items)
                    .ThenInclude(item => item.Product)
                .Include(order => order.Payment)
                .FirstOrDefaultAsync(order => order.Id == orderId && !order.IsDeleted);
        }
    }
}
