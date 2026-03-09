using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.AccountRequest
{
    public class AccountCreateRequest
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditLimit { get; set; } = 0;

        [Required]
        [StringLength(20)]
        public StatusAccount Status { get; set; } = StatusAccount.Active;

        public DateTime? LastRechargeDate { get; set; }
    }
}
