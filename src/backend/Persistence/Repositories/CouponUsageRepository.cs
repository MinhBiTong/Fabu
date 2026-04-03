using Domain.Entities;
using Domain.Repositories;
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
        public CouponUsageRepository(AppDbContext context) : base(context) {}

        public Task<int> CountUsageAsync(long couponId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(long customerId, long couponId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Coupon>> GetActiveCouponsForCustomerAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CouponUsage>> GetByCustomerAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<CouponUsage?> GetByTransactionIdAsync(long transactionId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CouponUsage>> GetRecentUsagesAsync(int top)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUsageCountByUserAsync(long customerId, long couponId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasUserUsedCouponAsync(long customerId, long couponId)
        {
            throw new NotImplementedException();
        }
    }
}
