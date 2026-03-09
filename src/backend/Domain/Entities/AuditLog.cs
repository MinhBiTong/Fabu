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
    public class AuditLog : EntityAuditBase<int>
    {
        public long? UserId { get; set; }

        public string Action { get; set; }

        public string EntityType { get; set; }

        public long? EntityId { get; set; }

        public string Description { get; set; }

        public string IpAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
