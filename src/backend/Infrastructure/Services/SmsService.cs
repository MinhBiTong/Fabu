using System.Text;
using System.Text.Json;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;

        public SmsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendSmsAsync(string phone, string message)
        {
            var body = new
            {
                ApiKey = "B49D17381C1A3A2606F5380E6D0B1F",
                SecretKey = "AAE0E51F1C8271841419198BFE91C4",
                Phone = phone, //dien thoai nguoi nhan
                Content = message, //noi dung tin nhan
                SmsType = 8 // 2 = CSKH/OTP
            };

            var json = new StringContent(JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "https://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/",
                json
            );

            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine(JsonSerializer.Serialize(body));
            Console.WriteLine(result);
        }
    }
}
