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
    public class PostpaidBill : EntityBase<int>
    {
        public long CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public DateTime BillMonth { get; set; }

        public DateTime DueDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; } = 0;

        public StatusPostpaid Status { get; set; } = StatusPostpaid.Unpaid;  // Unpaid, Partial, Paid, Overdue

        public virtual ICollection<Payment> Payments { get; set; }
        }
}
