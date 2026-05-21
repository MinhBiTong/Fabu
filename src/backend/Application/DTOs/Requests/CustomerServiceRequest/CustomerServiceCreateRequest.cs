using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.CustomerServiceRequest
{
    public class CustomerServiceCreateRequest
    {
        public long CustomerId { get; set; }

        public long ServiceId { get; set; }

        public DateTime ActivatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;

        public bool IsAutoRenewed { get; set; } // 0: No, 1: Yes
    }
}
