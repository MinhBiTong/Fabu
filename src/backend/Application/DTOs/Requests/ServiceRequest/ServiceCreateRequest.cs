using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.ServiceRequest
{
    public class ServiceCreateRequest
    {
        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; }

        public string ServiceCode { get; set; }

        public string Category { get; set; }

        public int DataAmountMB { get; set; }

        public short IsAutoRenew { get; set; } = 0; // 0: No, 1: Yes

        public int MaxActivationsPerMonth { get; set; } = 1;
        public decimal Price { get; set; }

        public int? ValidityDays { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
