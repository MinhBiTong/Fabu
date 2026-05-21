using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.TransactionRequest
{
    public class TransactionUpdateRequest
    {
        public long Id { get; set; }
        public StatusTransaction Status { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
