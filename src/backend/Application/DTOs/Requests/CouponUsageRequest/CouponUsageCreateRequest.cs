using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.CouponUsageRequest
{
    public class CouponUsageCreateRequest
    {
        public long CustomerId { get; set; }

        public long CouponId { get; set; }

        public long TransactionId { get; set; }

        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        public decimal DiscountApplied { get; set; } = 0;

    }
}
