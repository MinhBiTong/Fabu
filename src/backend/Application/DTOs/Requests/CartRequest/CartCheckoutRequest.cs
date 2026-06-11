using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.CartRequest
{
    public class CartCheckoutRequest
    {
        [Required]
        public long CustomerId { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.VNPay;

        public bool UseAccountBalance { get; set; }

        [StringLength(50)]
        public string? CouponCode { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        [StringLength(500)]
        public string? ShippingAddress { get; set; }

        [StringLength(1000)]
        public string? Note { get; set; }
    }
}
