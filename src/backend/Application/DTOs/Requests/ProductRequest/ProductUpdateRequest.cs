using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.ProductRequest
{
    public class ProductUpdateRequest
    {
        [Required, StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [StringLength(500)]
        public string? Tags { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? OriginalPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Range(0, 120)]
        public int WarrantyMonths { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; } = true;
    }
}
