using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RechargePlan : EntityBase<int>
    {
        public string PlanName { get; set; }

        public decimal Amount { get; set; }

        public decimal BonusAmount { get; set; } = 0;

        public int? ValidityDays { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
