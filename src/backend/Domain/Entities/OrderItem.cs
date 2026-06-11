using Domain.Abstractions;

namespace Domain.Entities
{
    public class OrderItem : EntityAuditSoftDeleteBase<long>
    {
        public Guid OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public long ProductId { get; set; }
        public virtual TelecomProduct? Product { get; set; }

        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal LineTotal { get; set; }
    }
}
