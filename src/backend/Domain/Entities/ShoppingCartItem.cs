using Domain.Abstractions;

namespace Domain.Entities
{
    public class ShoppingCartItem : EntityAuditSoftDeleteBase<long>
    {
        public long ShoppingCartId { get; set; }
        public virtual ShoppingCart? ShoppingCart { get; set; }

        public long ProductId { get; set; }
        public virtual TelecomProduct? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
