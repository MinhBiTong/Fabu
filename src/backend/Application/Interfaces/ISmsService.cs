using Application.DTOs.Responses.SmsResponse;

namespace Application.Interfaces
{
    public interface ISmsService
    {
        Task<SmsSendResult> SendSmsAsync(
            string phone,
            string smsMessage,
            CancellationToken cancellationToken = default);

        Task<SmsSendResult> SendOtpAsync(
            string phone,
            string otp,
            CancellationToken cancellationToken = default);
    }
}
