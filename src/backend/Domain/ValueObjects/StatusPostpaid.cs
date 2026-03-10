using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public enum StatusPostpaid
    {
        Unpaid = 0,
        Partial = 1,
        Paid = 2,
        Overdue = 3
    }
}
