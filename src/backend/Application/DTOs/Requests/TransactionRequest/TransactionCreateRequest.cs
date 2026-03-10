using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.TransactionRequest
{
    public class TransactionCreateRequest
    {
        public long CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [Required]
        [StringLength(30)]
        public string TransactionType { get; set; } // Recharge, BillPayment...

        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [StringLength(20)]
        public StatusTransaction Status { get; set; } = StatusTransaction.Success;

        [StringLength(50)]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Stripe;

        [StringLength(100)]
        public string TransactionRef { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
