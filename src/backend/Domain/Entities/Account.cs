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
    public class Account : EntityBase<long>
    {
        public long? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

        public decimal Balance { get; set; } = 0;

        public decimal CreditLimit { get; set; } = 0;

        public StatusAccount Status { get; set; } = StatusAccount.Active;

        public DateTime? LastRechargeDate { get; set; }

    }
}
