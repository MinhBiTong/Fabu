using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;

namespace Persistence.Repositories
{
    public class TelecomProductRepository : BaseRepository<TelecomProduct, long>, ITelecomProductRepository
    {
        public TelecomProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByCodeAsync(string productCode)
        {
            return await _dbSet.AnyAsync(product =>
                product.ProductCode == productCode && !product.IsDeleted);
        }

        public async Task<TelecomProduct?> GetByCodeAsync(string productCode)
        {
            return await _dbSet.FirstOrDefaultAsync(product =>
                product.ProductCode == productCode && !product.IsDeleted);
        }

        public async Task<List<TelecomProduct>> GetFeaturedAsync(int top)
        {
            return await _dbSet
                .Where(product => product.IsFeatured
                    && product.IsActive
                    && product.IsPublished
                    && !product.IsDeleted)
                .OrderByDescending(product => product.ModifiedDate)
                .Take(top)
                .ToListAsync();
        }

        public async Task<List<TelecomProduct>> SearchAsync(string? keyword, string? category, bool includeInactive = false)
        {
            var query = _dbSet.Where(product => !product.IsDeleted);

            if (!includeInactive)
            {
                query = query.Where(product => product.IsActive && product.IsPublished);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(product => product.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalized = keyword.Trim();
                query = query.Where(product =>
                    product.ProductName.Contains(normalized)
                    || product.ProductCode.Contains(normalized)
                    || product.Brand.Contains(normalized)
                    || product.Description.Contains(normalized)
                    || (product.Tags != null && product.Tags.Contains(normalized)));
            }

            return await query
                .OrderByDescending(product => product.IsFeatured)
                .ThenBy(product => product.ProductName)
                .ToListAsync();
        }
    }
}
