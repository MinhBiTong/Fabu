using Domain.Abstractions.Repositories;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICouponRepository : IRepositoryBase<Coupon, long>
    {
        Task<Coupon?> GetValidCouponByCodeAsync(string code, DateTime currentTime);
        Task<bool> IsCouponStillValidAsync(long couponId, long customerId);
        Task<List<Coupon>> GetActiveCouponAsync();
        Task<List<Coupon>> GetCouponsByCustomerIdAsync(long customerId);
        Task<bool> HasCustomerUsedCouponAsync(long couponId, long customerId);
        Task DecreaseUsageAsync(long couponId);
        Task<List<Coupon>> GetExpiredCouponAsync();
    }
}
