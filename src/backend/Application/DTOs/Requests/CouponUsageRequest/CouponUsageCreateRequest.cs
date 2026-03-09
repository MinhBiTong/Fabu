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
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public long CouponId { get; set; }
        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; }

        public long TransactionId { get; set; }
        [ForeignKey("TransactionId")]
        public virtual Transaction Transaction { get; set; }

        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        public decimal DiscountApplied { get; set; } = 0;

        public string Status { get; set; } = "Success";
    }
}
