using Application.DTOs.Requests.PaymentRequest;
using Application.Interfaces;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class StripeService : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly StripeConfiguration _configuration;
        private readonly ILogger<StripeService> _logger;

        public StripeService(HttpClient httpClient, IOptions<StripeConfiguration> configuration, ILogger<StripeService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration.Value;
            _logger = logger;
        }

        public async Task<string> CreatePaymentUrlAsync(PaymentCreateRequest request)
        {
            EnsureEnabled();

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _configuration.CheckoutSessionUrl);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.SecretKey);

            var successUrl = AppendQuery(_configuration.SuccessUrl, "paymentRef", request.PaymentRef ?? string.Empty);
            var form = new Dictionary<string, string>
            {
                ["mode"] = "payment",
                ["success_url"] = successUrl,
                ["cancel_url"] = _configuration.CancelUrl,
                ["client_reference_id"] = request.PaymentRef ?? string.Empty,
                ["metadata[paymentRef]"] = request.PaymentRef ?? string.Empty,
                ["line_items[0][quantity]"] = "1",
                ["line_items[0][price_data][currency]"] = _configuration.Currency,
                ["line_items[0][price_data][unit_amount]"] = Convert.ToInt64(request.Amount * 100).ToString(),
                ["line_items[0][price_data][product_data][name]"] = request.OrderInfo ?? $"Fabu payment {request.PaymentRef}"
            };

            httpRequest.Content = new FormUrlEncodedContent(form);
            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var checkoutUrl = document.RootElement.GetProperty("url").GetString();
            if (string.IsNullOrWhiteSpace(checkoutUrl))
                throw new InvalidOperationException("Stripe checkout URL was not returned.");

            _logger.LogInformation("Stripe checkout session created for payment {PaymentRef}", request.PaymentRef);
            return checkoutUrl;
        }

        public Task<PaymentCallbackResult> HandleCallbackAsync(Dictionary<string, string> callbackData)
        {
            var paymentRef = callbackData.GetValueOrDefault("paymentRef")
                ?? callbackData.GetValueOrDefault("client_reference_id");

            return Task.FromResult(string.IsNullOrWhiteSpace(paymentRef)
                ? PaymentCallbackResult.Failed("Stripe callback is missing paymentRef.")
                : PaymentCallbackResult.Success(paymentRef, GetProviderName(), callbackData));
        }

        public string GetProviderName() => "Stripe";

        private void EnsureEnabled()
        {
            if (!_configuration.Enabled || string.IsNullOrWhiteSpace(_configuration.SecretKey))
            {
                throw new InvalidOperationException("Stripe payment gateway is not configured.");
            }
        }

        private static string AppendQuery(string url, string key, string value)
        {
            var separator = url.Contains('?') ? "&" : "?";
            return $"{url}{separator}{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";
        }
    }
}
