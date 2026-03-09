using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.PostpaidRequest
{
    public class PostpaidCreateRequest
    {
        public long CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [Required]
        public DateTime BillMonth { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; } = 0;

        [Required]
        [StringLength(20)]
        public StatusPostpaid Status { get; set; } = StatusPostpaid.Unpaid;  // Unpaid, Partial, Paid, Overdue
    }
}
