namespace Application.Interfaces
{
    public interface ISmsService
    {
        Task<EsmsResponse> SendSmsAsync(string phone, string message);
    }
}
