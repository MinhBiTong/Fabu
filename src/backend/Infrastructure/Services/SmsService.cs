using System.Text;
using System.Text.Json;
using Application.Interfaces;
using Infrastructure.Models;

namespace Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;

        public SmsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EsmsResponse> SendSmsAsync(string phone, string message)
        {
            var body = new
            {
                ApiKey = "B49D17381C1A3A2606F5380E6D0B1F", //tai khoan
                SecretKey = "AAE0E51F1C8271841419198BFE91C4", //mat khau
                Phone = phone, //dien thoai nguoi nhan
                Content = message, //noi dung tin nhan
                //SmsType = 4 // 2 = CSKH/OTP
            };

            var json_body = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                "https://rest.esms.vn/MainService.svc/SendOtpMessage",
                new StringContent(json_body, Encoding.UTF8, "application/json")
            );

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Request: " + json_body);
            Console.WriteLine("Response: " + result);

            return JsonSerializer.Deserialize<EsmsResponse>(result);
        }
    }
}
