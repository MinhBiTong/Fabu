using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.CouponRequest
{
    public class CouponCreateRequest
    {
        [Table("Coupons")]
        public class Coupon
        {
            [Key]
            public int CouponId { get; set; }

            [Required]
            [StringLength(50)]
            public string Code { get; set; } // mã người dùng nhập

            [Required]
            [StringLength(150)]
            public string Name { get; set; } // "Tặng 30% data khi nạp 100k"

            public string Description { get; set; }

            [Required]
            [StringLength(20)]
            public string DiscountType { get; set; } // "Percentage", "FixedAmount", ...

            [Column(TypeName = "decimal(18,2)")]
            [Required]
            public decimal DiscountValue { get; set; } // 30 (%), 20000 (VND), 5000 (MB data)...

            [Column(TypeName = "decimal(18,2)")]
            public decimal MinRechargeAmount { get; set; } = 0; // áp dụng khi nạp từ X trở lên

            [Column(TypeName = "decimal(18,2)")]
            public decimal? MaxDiscount { get; set; } // giới hạn tối đa giảm

            [Required]
            public DateTime ValidFrom { get; set; }

            [Required]
            public DateTime ValidTo { get; set; }

            public int UsageLimitPerUser { get; set; } = 1; // mỗi user dùng được bao lần

            public int? TotalUsageLimit { get; set; } // tổng số lần dùng toàn hệ thống (NULL = unlimited)

            public bool IsActive { get; set; } = true; // default true

            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public long? CreatedBy { get; set; } // Admin tạo

            [ForeignKey("CreatedBy")]
            public virtual User CreatedByUser { get; set; } // Navigation property
        }
    }
}
