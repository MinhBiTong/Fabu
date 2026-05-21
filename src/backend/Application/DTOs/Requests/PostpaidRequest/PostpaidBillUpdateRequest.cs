using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.PostpaidRequest
{
    public class PostpaidBillUpdateRequest
    {
        public long Id { get; set; }
        public decimal PaidAmount { get; set; } = 0;

        [Required]
        [StringLength(20)]
        public StatusPostpaid Status { get; set; } = StatusPostpaid.Unpaid;  // Unpaid, Partial, Paid, Overdue
    }
}
