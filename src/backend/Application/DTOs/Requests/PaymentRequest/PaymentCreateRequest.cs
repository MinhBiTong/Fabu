using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.PaymentRequest
{
    public class PaymentCreateRequest
    {
        [Required]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Stripe;

        [StringLength(100)]
        public string? TransactionRef { get; set; }

        public long? BillId { get; set; }               // Nếu thanh toán hóa đơn postpaid

        public long? CustomerId { get; set; }           // Dùng khi guest thanh toán

        public string? IpAddress { get; set; }

        public string? OrderInfo { get; set; }
        public string? CouponCode { get; set; }
    }
}
