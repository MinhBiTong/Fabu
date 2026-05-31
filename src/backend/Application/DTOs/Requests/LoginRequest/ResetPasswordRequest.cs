using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.LoginRequest
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(256, MinimumLength = 8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
