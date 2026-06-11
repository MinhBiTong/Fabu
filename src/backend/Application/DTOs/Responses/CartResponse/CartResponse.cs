using Domain.Entities;

namespace Application.DTOs.Responses.CartResponse
{
    public class CartResponse
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public List<CartItemResponse> Items { get; set; } = new();

        public static CartResponse FromEntity(ShoppingCart cart)
        {
            var items = cart.Items?.Select(CartItemResponse.FromEntity).ToList() ?? new List<CartItemResponse>();
            return new CartResponse
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                Status = cart.Status.ToString(),
                Items = items,
                TotalItems = items.Sum(item => item.Quantity),
                TotalAmount = items.Sum(item => item.LineTotal)
            };
        }
    }

    public class CartItemResponse
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        public static CartItemResponse FromEntity(ShoppingCartItem item)
        {
            return new CartItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductCode = item.Product?.ProductCode ?? string.Empty,
                ProductName = item.Product?.ProductName ?? string.Empty,
                ImageUrl = item.Product?.ImageUrl,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.LineTotal
            };
        }
    }
}
