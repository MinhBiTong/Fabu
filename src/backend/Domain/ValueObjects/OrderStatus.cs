namespace Domain.ValueObjects
{
    public enum OrderStatus
    {
        Draft = 0,
        PendingPayment = 1,
        Paid = 2,
        Processing = 3,
        Completed = 4,
        Cancelled = 5,
        Failed = 6,
        Refunded = 7
    }
}
