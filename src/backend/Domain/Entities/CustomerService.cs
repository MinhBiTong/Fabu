
using Domain.Abstractions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class CustomerService : EntityAuditSoftDeleteBase<long>
    {
        public long CustomerId { get; set; }
        public long ServiceId { get; set; }

        public DateTime ActivatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsAutoRenewed { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service? Service { get; set; }
    }
}
