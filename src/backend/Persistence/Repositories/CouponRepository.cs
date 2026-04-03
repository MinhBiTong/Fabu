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
    public class CouponRepository : BaseRepository<Coupon, long>, ICouponRepository
    {
        public CouponRepository(AppDbContext context) : base(context) {}

        public Task DecreaseUsageAsync(long couponId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Coupon>> GetActiveCouponAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Coupon>> GetCouponsByCustomerIdAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Coupon>> GetExpiredCouponAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Coupon?> GetValidCouponByCodeAsync(string code, DateTime currentTime)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasCustomerUsedCouponAsync(long couponId, long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCouponStillValidAsync(long couponId, long customerId)
        {
            throw new NotImplementedException();
        }
    }
}
