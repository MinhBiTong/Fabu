using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICouponService
    {
        Task<string> GenerateCouponAsync(int userId, decimal discountAmount, DateTime expiryDate);
        Task<bool> ValidateCouponAsync(string couponCode, int userId);
        Task ConsumeCouponAsync(string couponCode, int userId);
    }
}
