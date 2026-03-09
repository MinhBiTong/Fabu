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
    public class Payment : EntityAuditBase<long>
    {
        public long? TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }

        public int? BillId { get; set; }
        public virtual PostpaidBill PostpaidBill { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Stripe;

        public string PaymentRef { get; set; }

        public StatusPayment Status { get; set; } = StatusPayment.Pending;
    }
}
