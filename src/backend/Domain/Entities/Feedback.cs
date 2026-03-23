using Domain.Abstractions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Feedback : EntityBase<int>
    {
        public long? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public byte Rating { get; set; }

        public StatusFeedback Status { get; set; } = StatusFeedback.New; // New, Read, Replied
    }
}
