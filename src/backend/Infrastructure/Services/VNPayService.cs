using Application.DTOs.Requests.PaymentRequest;
using Application.Interfaces;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Domain.Options;

namespace Infrastructure.Services
{
    public class VNPayService : IPaymentGateway
    {
        private readonly VNPayConfiguration _configs;
        private readonly ILogger<VNPayService> _logger;

        public VNPayService(IOptions<VNPayConfiguration> configs, ILogger<VNPayService> logger)
        {
            _configs = configs.Value;
            _logger = logger;
        }

        public async Task<string> CreatePaymentUrlAsync(PaymentCreateRequest request)
        {
            var vnpayData = new SortedList<string, string>
            {
                { "vnp_Version", _configs.Version },
                { "vnp_Command", _configs.Command },
                { "vnp_TmnCode", _configs.TmnCode },
                { "vnp_Amount", ((int)(request.Amount * 100)).ToString() },
                { "vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", _configs.CurrCode },
                { "vnp_IpAddr", request.IpAddress ?? "127.0.0.1" },
                { "vnp_Locale", _configs.Locale },
                { "vnp_OrderInfo", request.OrderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", _configs.ReturnUrl },
                { "vnp_TxnRef", request.TransactionRef }
            };

            var signData = string.Join("&", vnpayData.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));
            var secureHash = HmacSHA512(_configs.HashSecret, signData);

            vnpayData.Add("vnp_SecureHash", secureHash);

            var paymentUrl = $"{_configs.BaseUrl}/paymentv2/vpcpay.html?" +
                            string.Join("&", vnpayData.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));

            _logger.LogInformation("VNPay payment URL created for transaction {Ref}", request.TransactionRef);

            return paymentUrl;
        }

        public string GetProviderName() => "VNPay";

        public async Task<PaymentCallbackResult> HandleCallbackAsync(Dictionary<string, string> callbackData)
        {
            try
            {
                var vnpSecureHash = callbackData.GetValueOrDefault("vnp_SecureHash");
                callbackData.Remove("vnp_SecureHash");
                var signData = string.Join("&", callbackData.OrderBy(x => x.Key)
                    .Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));

                var calculatedHash = HmacSHA512(_configs.HashSecret, signData);

                if (vnpSecureHash != calculatedHash)
                    return PaymentCallbackResult.Failed( "Invalid signature" );

                var responseCode = callbackData.GetValueOrDefault("vnp_ResponseCode");
                var txnRef = callbackData.GetValueOrDefault("vnp_TxnRef");

                if (responseCode == "00")
                {
                    return PaymentCallbackResult.Success(txnRef, "VNPay", callbackData);
                }
                return PaymentCallbackResult.Failed($"Payment failed with code: {responseCode}");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error handling VNPay callback");
                return PaymentCallbackResult.Failed("Error processing callback");
            }
        }

        private static string HmacSHA512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
