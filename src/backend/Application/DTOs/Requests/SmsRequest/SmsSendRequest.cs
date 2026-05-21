using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.SmsRequest;

public sealed class SmsSendRequest
{
    [Required]
    [StringLength(20, MinimumLength = 9)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Message { get; set; } = string.Empty;
}
