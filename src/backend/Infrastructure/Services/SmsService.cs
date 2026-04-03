using System.Text;
using System.Text.Json;
using Application.DTOs.Responses.SmsResponse;
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

        public async Task<SmsResult> SendSmsAsync(string phone, string message)
        {
            var body = new
            {
                ApiKey = "B49D17381C1A3A2606F5380E6D0B1F", //tai khoan
                SecretKey = "AAE0E51F1C8271841419198BFE91C4", //mat khau
                Phone = phone, //dien thoai nguoi nhan
                Content = message, //noi dung tin nhan
                SmsType = 2 // 2 = CSKH/OTP
            };

            var json_body = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                "https://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/",
                new StringContent(json_body, Encoding.UTF8, "application/json")
            );

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Request: " + json_body);
            Console.WriteLine("Response: " + result);

            // Parse response
            var esms = JsonSerializer.Deserialize<EsmsResponse>(result);

            if (esms == null)
            {
                return new SmsResult
                {
                    Success = false,
                    Message = "Không parse được response từ eSMS"
                };
            }

            return new SmsResult
            {
                Success = esms.CodeResult == "100",
                Message = esms.CodeResult == "100"
                    ? "Gửi SMS thành công"
                    : esms.ErrorMessage,
                SmsId = esms.SMSID
            };
        }
    }
}
