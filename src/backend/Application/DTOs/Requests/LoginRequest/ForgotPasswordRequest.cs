using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.LoginRequest
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
    }
}
