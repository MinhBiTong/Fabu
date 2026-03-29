using System;

namespace Application.DTOs.Responses
{
    public class CustomerServiceResponse
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long ServiceId { get; set; }
        public DateTime ActivatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsAutoRenewed { get; set; }
    }
}