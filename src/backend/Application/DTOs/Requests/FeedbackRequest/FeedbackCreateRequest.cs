using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.FeedbackRequest
{
    public class FeedbackCreateRequest
    {
        public long? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public long? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [StringLength(200)]
        public string Subject { get; set; }

        public string Message { get; set; }

        [Range(1, 5)]
        public byte Rating { get; set; }

        [StringLength(20)]
        public StatusFeedback Status { get; set; } = StatusFeedback.New; // New, Read, Replied
    }
}
