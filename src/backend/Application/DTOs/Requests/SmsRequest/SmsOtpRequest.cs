using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.SmsRequest;

public sealed class SmsOtpRequest
{
    [Required]
    [StringLength(20, MinimumLength = 9)]
    public string Phone { get; set; } = string.Empty;
}
