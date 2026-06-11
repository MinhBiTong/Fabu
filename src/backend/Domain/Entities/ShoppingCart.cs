using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class ShoppingCart : EntityAuditSoftDeleteBase<long>
    {
        public long CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public ShoppingCartStatus Status { get; set; } = ShoppingCartStatus.Active;
        public DateTimeOffset? CheckedOutAt { get; set; }

        public virtual ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    }
}
