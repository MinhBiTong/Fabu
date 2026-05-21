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

        [Required]
        public DateTime BillMonth { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public decimal TotalAmount { get; set; }

    }
}
