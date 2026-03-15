using Domain.Abstractions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Coupon : EntityBase<long>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public DiscountType DiscountType { get; set; } = DiscountType.Percentage; // Percentage, FixedAmount...

        public decimal DiscountValue { get; set; }

        public decimal MinRechargeAmount { get; set; }

        public decimal MaxDiscount { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public int UsageLimitPerUser { get; set; } = 1;

        public int UsageLimitTotal { get; set; } = 1000;
        
        public bool IsActive { get; set; } = true;

        public long? CreatedByUserId { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public virtual ICollection<CouponUsage> CouponUsages { get; set; }
    }
}
