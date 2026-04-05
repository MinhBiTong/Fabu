using Application.DTOs.Responses.CouponResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICouponService
    {
        Task<CouponApplyResult> ApplyCouponAsync(string couponCode, long customerId, decimal originalAmount, string transactionType);
        Task<string> GenerateCouponAsync(int userId, decimal discountAmount, DateTime expiryDate);
        Task<bool> ValidateCouponAsync(string couponCode, int userId);
        Task ConsumeCouponAsync(string couponCode, int userId);
    }
}
