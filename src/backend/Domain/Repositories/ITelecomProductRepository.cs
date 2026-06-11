using Domain.Abstractions.Repositories;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface ITelecomProductRepository : IRepositoryBase<TelecomProduct, long>
    {
        Task<TelecomProduct?> GetByCodeAsync(string productCode);
        Task<List<TelecomProduct>> SearchAsync(string? keyword, string? category, bool includeInactive = false);
        Task<List<TelecomProduct>> GetFeaturedAsync(int top);
        Task<bool> ExistsByCodeAsync(string productCode);
    }
}
