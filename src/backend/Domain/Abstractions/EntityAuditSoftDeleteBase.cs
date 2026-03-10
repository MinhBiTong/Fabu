using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public abstract class EntityAuditSoftDeleteBase<TKey> : EntityAuditBase<TKey>, ISoftDelete where TKey : struct
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset DeletedAt { get; set; }
        public virtual void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTimeOffset.UtcNow;
        }
    }
}
