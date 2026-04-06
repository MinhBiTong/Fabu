using Application.DTOs.Responses.CouponResponse;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CouponService : ICouponService
    {
        public Task<CouponApplyResult> ApplyCouponAsync(string couponCode, long customerId, decimal originalAmount, string transactionType)
        {
            throw new NotImplementedException();
        }

        public Task ConsumeCouponAsync(string couponCode, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateCouponAsync(int userId, decimal discountAmount, DateTime expiryDate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateCouponAsync(string couponCode, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
