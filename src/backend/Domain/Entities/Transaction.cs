using Domain.Abstractions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Transaction: EntityAuditBase<long>
    {
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public string TransactionType { get; set; } // Recharge, BillPayment...

        public decimal Amount { get; set; }

        public StatusTransaction Status { get; set; } = StatusTransaction.Success;

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Stripe;

        public string TransactionRef { get; set; }

        public DateTime? CompletedAt { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<CouponUsage> Coupons { get; set; }
    }
}
