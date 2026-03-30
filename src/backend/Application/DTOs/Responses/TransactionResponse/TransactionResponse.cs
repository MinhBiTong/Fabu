using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses.TransactionResponse
{
    public class TransactionResponse
    {
        public long CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [Required]
        [StringLength(30)]
        public string TransactionType { get; set; } // Recharge, BillPayment...

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [StringLength(20)]
        public StatusTransaction Status { get; set; } = StatusTransaction.Success;

        [StringLength(50)]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Stripe;

        [StringLength(100)]
        public string TransactionRef { get; set; }

        public DateTime? CompletedAt { get; set; }

        public virtual ICollection<CouponUsage> CouponUsages { get; set; }
        public static TransactionResponse FromEntity(Transaction entity)
        {
            if (entity == null) return null;
            return new TransactionResponse
            {
                TransactionRef = entity.TransactionRef,
                Amount = entity.Amount,
                TransactionType = entity.TransactionType,
                Status = entity.Status,
                PaymentMethod = entity.PaymentMethod,
                CompletedAt = entity.CompletedAt,
            };
        }
    }
}
