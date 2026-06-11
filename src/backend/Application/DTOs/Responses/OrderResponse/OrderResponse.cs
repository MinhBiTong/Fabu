using Domain.Entities;

namespace Application.DTOs.Responses.OrderResponse
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public long CustomerId { get; set; }
        public long? PaymentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? CouponCode { get; set; }
        public string? ContactPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Note { get; set; }
        public DateTimeOffset? PaidAt { get; set; }
        public string? PaymentRef { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();

        public static OrderResponse FromEntity(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerId = order.CustomerId,
                PaymentId = order.PaymentId,
                Status = order.Status.ToString(),
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod.ToString(),
                CouponCode = order.CouponCode,
                ContactPhone = order.ContactPhone,
                ShippingAddress = order.ShippingAddress,
                Note = order.Note,
                PaidAt = order.PaidAt,
                PaymentRef = order.Payment?.PaymentRef,
                Items = order.Items?.Select(OrderItemResponse.FromEntity).ToList() ?? new List<OrderItemResponse>()
            };
        }
    }

    public class OrderItemResponse
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal LineTotal { get; set; }

        public static OrderItemResponse FromEntity(OrderItem item)
        {
            return new OrderItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductCode = item.ProductCode,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountAmount = item.DiscountAmount,
                LineTotal = item.LineTotal
            };
        }
    }
}
