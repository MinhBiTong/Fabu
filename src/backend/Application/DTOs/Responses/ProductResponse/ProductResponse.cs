using Domain.Entities;

namespace Application.DTOs.Responses.ProductResponse
{
    public class ProductResponse
    {
        public long Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Tags { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int StockQuantity { get; set; }
        public int WarrantyMonths { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; }

        public static ProductResponse FromEntity(TelecomProduct product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Category = product.Category,
                Brand = product.Brand,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Tags = product.Tags,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                StockQuantity = product.StockQuantity,
                WarrantyMonths = product.WarrantyMonths,
                IsActive = product.IsActive,
                IsFeatured = product.IsFeatured,
                IsPublished = product.IsPublished
            };
        }
    }
}
