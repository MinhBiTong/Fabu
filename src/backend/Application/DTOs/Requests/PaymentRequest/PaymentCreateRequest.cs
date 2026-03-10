using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.PaymentRequest
{
    public class PaymentCreateRequest
    {
        public long? TransactionId { get; set; }
        [ForeignKey("TransactionId")]
        public virtual Transaction Transaction { get; set; }

        public long? BillId { get; set; }
        [ForeignKey("BillId")]
        public virtual PostpaidBill PostpaidBill { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Stripe;

        [StringLength(100)]
        public string PaymentRef { get; set; }

        [StringLength(20)]
        public StatusPayment Status { get; set; } = StatusPayment.Pending;
    }
}
