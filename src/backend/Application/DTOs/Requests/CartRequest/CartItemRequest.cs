using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.CartRequest
{
    public class CartItemRequest
    {
        [Required]
        public long CustomerId { get; set; }

        [Required]
        public long ProductId { get; set; }

        [Range(1, 999)]
        public int Quantity { get; set; } = 1;
    }
}
