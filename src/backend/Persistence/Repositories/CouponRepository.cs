using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;

namespace Persistence.Repositories
{
    public class CouponRepository : BaseRepository<Coupon, long>, ICouponRepository
    {
        public CouponRepository(AppDbContext context) : base(context) {}

        public async Task DecreaseUsageAsync(long couponId)
        {
            var coupon = await _dbSet
                .Include(x => x.CouponUsages)
                .FirstOrDefaultAsync(x => x.Id == couponId);

            if (coupon == null)
                throw new Exception("Coupon not found");

            if (coupon.CouponUsages.Count >= coupon.UsageLimitTotal)
                throw new Exception("Coupon hết lượt sử dụng");

            // Khong can giam gi → vi usage duoc tinh bang COUNT
        }

        public async Task<List<Coupon>> GetActiveCouponAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(x => x.IsActive && x.ValidFrom <= now && x.ValidTo >= now)
                .ToListAsync();
        }

        public async Task<List<Coupon>> GetCouponsByCustomerIdAsync(long customerId)
        {
            return await _dbSet
                .Include(x => x.CouponUsages)
                .ThenInclude(u => u.Transaction)
                .Where(x => x.CouponUsages.Any(u =>
                    u.Transaction != null &&
                    u.Transaction.CustomerId == customerId
                )).ToListAsync();
        }

        public async Task<List<Coupon>> GetExpiredCouponAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(x => x.ValidTo < now || !x.IsActive)
                .ToListAsync();
        }

        public async Task<Coupon?> GetValidCouponByCodeAsync(string code, DateTime currentTime)
        {
            return await _dbSet
                .Include(x => x.CouponUsages)
                .FirstOrDefaultAsync(x =>
                    x.Code == code &&
                    x.IsActive &&
                    x.ValidFrom <= currentTime &&
                    x.ValidTo >= currentTime &&
                    x.UsageLimitTotal > x.CouponUsages.Count
                );
        }

        public async Task<bool> HasCustomerUsedCouponAsync(long couponId, long customerId)
        {
            return await _context.Set<CouponUsage>()
                .Include(x => x.Transaction)
                .AnyAsync(x =>
                    x.CouponId == couponId &&
                    x.Transaction != null &&
                    x.Transaction.CustomerId == customerId &&
                    x.Status == "Success"
                );
        }

        public async Task<bool> IsCouponStillValidAsync(long couponId, long customerId)
        {
            var coupon = await _dbSet
                .Include(x => x.CouponUsages)
                .ThenInclude(u => u.Transaction)
                .FirstOrDefaultAsync(x => x.Id == couponId);

            if (coupon == null) return false;

            var now = DateTime.UtcNow;

            //kiem tra thoi gian + active
            if (!coupon.IsActive || now < coupon.ValidFrom || now > coupon.ValidTo)
                return false;

            //kiem tra tong usage (chi success)
            var totalUsed = coupon.CouponUsages.Count(x => x.Status == "Success");

            if (totalUsed >= coupon.UsageLimitTotal)
                return false;

            //kiem tra per user
            var userUsage = coupon.CouponUsages
                .Count(x =>
                    x.Status == "Success" &&
                    x.Transaction != null &&
                    x.Transaction.CustomerId == customerId
                );

            if (userUsage >= coupon.UsageLimitPerUser)
                return false;

            return true;
        }
    }
}
