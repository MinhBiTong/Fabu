using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Order : EntityAuditSoftDeleteBase<Guid>
    {
        public long CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

        public long? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }

        public string OrderCode { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.VNPay;
        public string? CouponCode { get; set; }
        public string? ContactPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Note { get; set; }
        public DateTimeOffset? PaidAt { get; set; }
        public DateTimeOffset? CancelledAt { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
