using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CouponUsage : EntityAuditBase<long>
    {
        public long? CouponId { get; set; }
        public virtual Coupon? Coupon { get; set; }

        public long? TransactionId { get; set; }
        public virtual Transaction? Transaction { get; set; }

        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        public decimal DiscountApplied { get; set; } = 0;

        public string Status { get; set; } = "Success"; 
    }
}
