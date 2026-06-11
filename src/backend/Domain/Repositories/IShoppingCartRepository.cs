using Domain.Abstractions.Repositories;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IShoppingCartRepository : IRepositoryBase<ShoppingCart, long>
    {
        Task<ShoppingCart?> GetActiveCartByCustomerAsync(long customerId);
        Task<ShoppingCart?> GetCartWithItemsAsync(long cartId);
    }
}
