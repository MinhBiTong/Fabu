using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.PaymentRequest
{
    public class PackagePaymentRequest
    {
        [Required]
        public long CustomerId { get; set; }

        [Required]
        public long ServiceId { get; set; }

        [Range(1, 36)]
        public int SubscriptionMonths { get; set; } = 1;

        public bool PayMonthly { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.VNPay;
        public bool UseAccountBalance { get; set; }
        public string? CouponCode { get; set; }
    }
}
