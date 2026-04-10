using Domain.Abstractions.Repositories;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICouponUsageRepository : IRepositoryBase<CouponUsage, long>
    {
        Task<bool> HasUserUsedCouponAsync(long customerId, long couponId);
        Task<IEnumerable<Coupon>> GetActiveCouponsForCustomerAsync(long customerId);
        Task<int> GetUsageCountByUserAsync(long customerId, long couponId);
        Task<CouponUsage?> GetByTransactionIdAsync(long transactionId);
        Task<List<CouponUsage>> GetByCustomerAsync(long customerId);
        Task<bool> ExistsAsync(long customerId, long couponId);
        Task<int> CountUsageAsync(long couponId);
        Task<List<CouponUsage>> GetRecentUsagesAsync(int top);
    }
}
