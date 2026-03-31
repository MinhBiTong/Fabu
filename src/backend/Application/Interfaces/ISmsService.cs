namespace Application.Interfaces
{
    public interface ISmsService
    {
        Task SendSmsAsync(string phone, string sms_message);
    }
}
