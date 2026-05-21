using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Options
{
    public class PaymentCallbackResult
    {
        public bool IsSuccess { get; set; }
        public string PaymentRef { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string> RawData { get; set; } = new();

        public static PaymentCallbackResult Success(string transactionRef, string provider, Dictionary<string, string> rawData)
            => new() { IsSuccess = true, PaymentRef = transactionRef, Provider = provider, RawData = rawData };

        public static PaymentCallbackResult Failed(string message)
            => new() { IsSuccess = false, Message = message };
    }
}
