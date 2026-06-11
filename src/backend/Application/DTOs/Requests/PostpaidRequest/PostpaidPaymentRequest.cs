using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.PostpaidRequest
{
    public class PostpaidPaymentRequest
    {
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.VNPay;
        public bool UseAccountBalance { get; set; }
        public string? CouponCode { get; set; }
    }
}
