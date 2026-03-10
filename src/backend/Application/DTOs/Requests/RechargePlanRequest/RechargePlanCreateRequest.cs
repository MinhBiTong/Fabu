using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.RechargePlanRequest
{
    public class RechargePlanCreateRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string PlanName { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        public decimal BonusAmount { get; set; } = 0;

        public int? ValidityDays { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
