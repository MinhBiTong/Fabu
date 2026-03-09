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
    public class Service : EntityBase<long>
    {
        public string ServiceName { get; set; }

        public string ServiceCode { get; set; }

        public string Category { get; set; }

        public int DataAmountMB { get; set; }
        
        public bool IsAutoRenew { get; set; }  // 0: No, 1: Yes

        public int MaxActivationsPerMonth { get; set; } = 1;
        public decimal Price { get; set; }

        public int? ValidityDays { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<CustomerService> CustomerServices { get; set; }
        }
}
