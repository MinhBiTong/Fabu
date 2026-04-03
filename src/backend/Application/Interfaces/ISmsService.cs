using Application.DTOs.Responses.SmsResponse;

namespace Application.Interfaces
{
    public interface ISmsService
    {
        Task<SmsResult> SendSmsAsync(string phone, string message);
    }
}
