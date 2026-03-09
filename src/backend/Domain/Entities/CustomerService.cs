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
    public class CustomerService : EntityAuditBase<int>
    {
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public long ServiceId { get; set; }
        public virtual Service Service { get; set; }

        public DateTime ActivatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;

        public bool IsAutoRenewed { get; set; } // 0: No, 1: Yes
    }
}
