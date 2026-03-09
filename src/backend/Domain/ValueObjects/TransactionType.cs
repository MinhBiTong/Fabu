using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class TransactionType
    {
        public const string Recharge = "Recharge";
        public const string BillPayment = "BillPayment";
        public const string ServiceActivation = "ServiceActivation";
        public const string Refund = "Refund";
    }
}
