using Domain.Entities;

namespace Application.DTOs.Responses.TransactionResponse
{
    public class TransactionResponse
    {
        public long? CustomerId { get; set; }
        public long? PaymentId { get; set; }
        public Guid? OrderId { get; set; }
        public long? ServiceId { get; set; }
        public int? SubscriptionMonths { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionRef { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public int CouponUsageCount { get; set; }

        public static TransactionResponse FromEntity(Transaction entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new TransactionResponse
            {
                CustomerId = entity.CustomerId,
                PaymentId = entity.PaymentId,
                OrderId = entity.OrderId,
                ServiceId = entity.ServiceId,
                SubscriptionMonths = entity.SubscriptionMonths,
                TransactionRef = entity.TransactionRef,
                Amount = entity.Amount,
                TransactionType = entity.TransactionType,
                Status = entity.Status.ToString(),
                PaymentMethod = entity.PaymentMethod.ToString(),
                CompletedAt = entity.CompletedAt,
                CouponUsageCount = entity.CouponUsages?.Count ?? 0
            };
        }
    }
}
