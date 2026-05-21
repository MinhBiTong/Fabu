using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.AuditLogRequest
{
    public class AuditLogCreateRequest
    {
        [StringLength(100)]
        public string Action { get; set; }

        [StringLength(50)]
        public string EntityType { get; set; }

        public long? EntityId { get; set; }

        public string Description { get; set; }

        [StringLength(45)]
        public string IpAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
