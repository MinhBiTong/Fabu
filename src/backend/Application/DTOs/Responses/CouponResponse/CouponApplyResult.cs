using Domain.Entities;

namespace Application.DTOs.Responses.CouponResponse
{
    public class CouponApplyResult
    {
        public bool IsSuccess { get; set; }
        public decimal FinalAmount { get; set; }
        public string Message { get; set; } = string.Empty;
        public CouponUsage? CouponUsage { get; set; }

        public static CouponApplyResult Success(decimal finalAmount, CouponUsage couponUsage)
            => new() { IsSuccess = true, FinalAmount = finalAmount, CouponUsage = couponUsage };

        public static CouponApplyResult Failed(string message)
            => new() { IsSuccess = false, Message = message };
    }
}
