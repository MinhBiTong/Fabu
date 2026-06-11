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

        [StringLength(50)]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Stripe;

        [StringLength(100)]
        public string? PaymentRef { get; set; }

        public long? BillId { get; set; }               // Nếu thanh toán hóa đơn postpaid
        public long? CustomerId { get; set; }
        public Guid? OrderId { get; set; }
        public long? ServiceId { get; set; }

        [Range(1, 36)]
        public int SubscriptionMonths { get; set; } = 1;

        [StringLength(50)]
        public string? MobileNumber { get; set; }

        [StringLength(30)]
        public string TransactionType { get; set; } = "Recharge";

        public bool UseAccountBalance { get; set; }

        public string? IpAddress { get; set; }

        public string? OrderInfo { get; set; }
        public string? CouponCode { get; set; }
    }
}
