using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;

namespace Persistence.Repositories
{
    public class ShoppingCartRepository : BaseRepository<ShoppingCart, long>, IShoppingCartRepository
    {
        public ShoppingCartRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ShoppingCart?> GetActiveCartByCustomerAsync(long customerId)
        {
            return await _dbSet
                .Include(cart => cart.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(cart =>
                    cart.CustomerId == customerId
                    && cart.Status == ShoppingCartStatus.Active
                    && !cart.IsDeleted);
        }

        public async Task<ShoppingCart?> GetCartWithItemsAsync(long cartId)
        {
            return await _dbSet
                .Include(cart => cart.Customer)
                .Include(cart => cart.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(cart => cart.Id == cartId && !cart.IsDeleted);
        }
    }
}
