using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class CouponUsageRepository : BaseRepository<CouponUsage, long>, ICouponUsageRepository
    {
        public CouponUsageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<int> CountUsageAsync(long couponId)
        {
            return await _dbSet.CountAsync(x => x.CouponId == couponId);
        }

        public async Task<bool> ExistsAsync(long customerId, long couponId)
        {
            return await _dbSet
                .Include(x => x.Transaction)
                .AnyAsync(x =>
                    x.CouponId == couponId &&
                    x.Transaction != null &&
                    x.Transaction.CustomerId == customerId
                );
        }

        public async Task<IEnumerable<Coupon>> GetActiveCouponsForCustomerAsync(long customerId)
        {
            var now = DateTime.UtcNow;

            return await _context.Set<Coupon>()
                .Where(c =>
                    c.IsActive &&
                    c.ValidFrom <= now &&
                    c.ValidTo >= now &&
                    !c.CouponUsages.Any(u =>
                        u.Transaction != null &&
                        u.Transaction.CustomerId == customerId))
                .ToListAsync();
        }

        public async Task<List<CouponUsage>> GetByCustomerAsync(long customerId)
        {
            return await _dbSet
                .Include(x => x.Transaction)
                .Where(x =>
                    x.Transaction != null &&
                    x.Transaction.CustomerId == customerId)
                .OrderByDescending(x => x.UsedAt)
                .ToListAsync();
        }

        public async Task<CouponUsage?> GetByTransactionIdAsync(long transactionId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        }

        public async Task<List<CouponUsage>> GetRecentUsagesAsync(int top)
        {
            return await _dbSet
                .OrderByDescending(x => x.UsedAt)
                .Take(top)
                .ToListAsync();
        }

        public async Task<int> GetUsageCountByUserAsync(long customerId, long couponId)
        {
            return await _dbSet
                .Include(x => x.Transaction)
                .CountAsync(x =>
                    x.CouponId == couponId &&
                    x.Transaction != null &&
                    x.Transaction.CustomerId == customerId
                );
        }

        public async Task<bool> HasUserUsedCouponAsync(long customerId, long couponId)
        {
            return await _dbSet
                .Include(x => x.Transaction)
                .AnyAsync(x =>
                    x.CouponId == couponId &&
                    x.Transaction != null &&
                    x.Transaction.CustomerId == customerId
                );
        }
    }
}
