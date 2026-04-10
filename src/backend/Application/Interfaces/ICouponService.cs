using Application.DTOs.Requests.CouponRequest;
using Application.DTOs.Responses.CouponResponse;

namespace Application.Interfaces
{
    public interface ICouponService
    {
        Task<CouponApplyResult> ApplyCouponAsync(string couponCode, long customerId, decimal originalAmount, string transactionType);
        Task<string> GenerateCouponAsync(int userId, decimal discountAmount, DateTime expiryDate);
        Task<bool> ValidateCouponAsync(string couponCode, int userId);
        Task ConsumeCouponAsync(string couponCode, int userId);

        //Task<CouponResponse> CreateCouponAsync(CouponCreateRequest request);
        //Task<CouponResponse> UpdateCouponAsync(long id, CouponUpdateRequest request);
        //Task DeleteCouponAsync(long id);

    }
}
