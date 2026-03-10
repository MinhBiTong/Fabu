using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Customer : EntityAuditSoftDeleteBase<long>
    {
        public string MobileNumber { get; set; }

        public string CustomerType { get; set; } // 'Prepaid' or 'Postpaid'

        public string FullName { get; set; }

        public string Address { get; set; }

        public long? UserId { get; set; }
        public virtual User User { get; set; }
        public long? AccountId { get; set; }
        public virtual Account Account { get; set; }
        public virtual ICollection<PostpaidBill> PostpaidBills { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<CustomerService> CustomerServices { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<CouponUsage> CouponUsages { get; set; }
    }
}
