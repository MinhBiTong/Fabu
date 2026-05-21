using Application.DTOs.Requests.PaymentRequest;
using Application.Interfaces;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class PayPalService : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly PayPalConfiguration _configuration;
        private readonly ILogger<PayPalService> _logger;

        public PayPalService(HttpClient httpClient, IOptions<PayPalConfiguration> configuration, ILogger<PayPalService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration.Value;
            _logger = logger;
        }

        public async Task<string> CreatePaymentUrlAsync(PaymentCreateRequest request)
        {
            EnsureEnabled();

            var accessToken = await GetAccessTokenAsync();
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_configuration.BaseUrl.TrimEnd('/')}/v2/checkout/orders");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var returnUrl = AppendQuery(_configuration.ReturnUrl, "paymentRef", request.PaymentRef ?? string.Empty);
            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        reference_id = request.PaymentRef,
                        custom_id = request.PaymentRef,
                        amount = new
                        {
                            currency_code = _configuration.Currency,
                            value = request.Amount.ToString("0.00")
                        },
                        description = request.OrderInfo ?? $"Fabu payment {request.PaymentRef}"
                    }
                },
                application_context = new
                {
                    return_url = returnUrl,
                    cancel_url = _configuration.CancelUrl
                }
            };

            httpRequest.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var approvalUrl = document.RootElement
                .GetProperty("links")
                .EnumerateArray()
                .FirstOrDefault(link =>
                    link.TryGetProperty("rel", out var rel) &&
                    rel.GetString() == "approve")
                .GetProperty("href")
                .GetString();

            if (string.IsNullOrWhiteSpace(approvalUrl))
                throw new InvalidOperationException("PayPal approval URL was not returned.");

            _logger.LogInformation("PayPal order created for payment {PaymentRef}", request.PaymentRef);
            return approvalUrl;
        }

        public async Task<PaymentCallbackResult> HandleCallbackAsync(Dictionary<string, string> callbackData)
        {
            EnsureEnabled();

            var orderId = callbackData.GetValueOrDefault("token");
            var paymentRef = callbackData.GetValueOrDefault("paymentRef");

            if (string.IsNullOrWhiteSpace(orderId) || string.IsNullOrWhiteSpace(paymentRef))
                return PaymentCallbackResult.Failed("PayPal callback is missing token or paymentRef.");

            try
            {
                var accessToken = await GetAccessTokenAsync();
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration.BaseUrl.TrimEnd('/')}/v2/checkout/orders/{orderId}/capture");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return PaymentCallbackResult.Success(paymentRef, GetProviderName(), callbackData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PayPal callback failed for payment {PaymentRef}", paymentRef);
                return PaymentCallbackResult.Failed("PayPal capture failed.");
            }
        }

        public string GetProviderName() => "PayPal";

        private async Task<string> GetAccessTokenAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration.BaseUrl.TrimEnd('/')}/v1/oauth2/token");
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_configuration.ClientId}:{_configuration.ClientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            });

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return document.RootElement.GetProperty("access_token").GetString()
                ?? throw new InvalidOperationException("PayPal access token was not returned.");
        }

        private void EnsureEnabled()
        {
            if (!_configuration.Enabled ||
                string.IsNullOrWhiteSpace(_configuration.ClientId) ||
                string.IsNullOrWhiteSpace(_configuration.ClientSecret))
            {
                throw new InvalidOperationException("PayPal payment gateway is not configured.");
            }
        }

        private static string AppendQuery(string url, string key, string value)
        {
            var separator = url.Contains('?') ? "&" : "?";
            return $"{url}{separator}{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";
        }
    }
}
