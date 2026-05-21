using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Application.DTOs.Responses.SmsResponse;
using Application.Interfaces;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public sealed class SmsService : ISmsService
    {
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
        private static readonly Regex PhoneRegex = new("^84\\d{8,11}$", RegexOptions.Compiled);

        private readonly HttpClient _httpClient;
        private readonly SmsConfiguration _configuration;
        private readonly ILogger<SmsService> _logger;

        public SmsService(
            HttpClient httpClient,
            IOptions<SmsConfiguration> options,
            ILogger<SmsService> logger)
        {
            _httpClient = httpClient;
            _configuration = options.Value;
            _logger = logger;
            _httpClient.Timeout = TimeSpan.FromSeconds(Math.Clamp(_configuration.TimeoutSeconds, 3, 60));
        }

        public Task<SmsSendResult> SendOtpAsync(
            string phone,
            string otp,
            CancellationToken cancellationToken = default)
        {
            var message = _configuration.OtpTemplate.Replace("{otp}", otp, StringComparison.OrdinalIgnoreCase);
            return SendSmsAsync(phone, message, cancellationToken);
        }

        public async Task<SmsSendResult> SendSmsAsync(
            string phone,
            string smsMessage,
            CancellationToken cancellationToken = default)
        {
            var normalizedPhone = NormalizeVietnamPhone(phone);
            if (normalizedPhone is null)
            {
                return SmsSendResult.Failure(ResolveProviderName(), phone, "So dien thoai khong hop le.");
            }

            if (string.IsNullOrWhiteSpace(smsMessage))
            {
                return SmsSendResult.Failure(ResolveProviderName(), normalizedPhone, "Noi dung SMS khong duoc de trong.");
            }

            if (smsMessage.Length > Math.Max(1, _configuration.MaxMessageLength))
            {
                return SmsSendResult.Failure(
                    ResolveProviderName(),
                    normalizedPhone,
                    $"Noi dung SMS vuot qua {_configuration.MaxMessageLength} ky tu.");
            }

            if (!_configuration.Enabled ||
                _configuration.Provider.Equals("Mock", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation(
                    "SMS mock send. Phone: {Phone}, Length: {Length}",
                    MaskPhone(normalizedPhone),
                    smsMessage.Length);

                return SmsSendResult.Success(
                    "Mock",
                    normalizedPhone,
                    $"mock-{RandomNumberGenerator.GetInt32(100000, 999999)}",
                    "MOCK",
                    "SMS provider is disabled; mock result returned.",
                    null,
                    0);
            }

            if (string.IsNullOrWhiteSpace(_configuration.ApiKey) ||
                string.IsNullOrWhiteSpace(_configuration.SecretKey))
            {
                _logger.LogError("SMS provider is enabled but ApiKey/SecretKey is missing.");
                return SmsSendResult.Failure(ResolveProviderName(), normalizedPhone, "SMS provider chua duoc cau hinh key.");
            }

            var attempts = Math.Clamp(_configuration.MaxRetryAttempts, 1, 5);
            SmsSendResult? lastResult = null;

            for (var attempt = 1; attempt <= attempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    lastResult = await SendWithESmsAsync(
                        normalizedPhone,
                        smsMessage,
                        attempt,
                        cancellationToken);

                    if (lastResult.IsSuccess || attempt == attempts)
                    {
                        return lastResult;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "SMS send attempt {Attempt}/{MaxAttempts} failed. Phone: {Phone}",
                        attempt,
                        attempts,
                        MaskPhone(normalizedPhone));

                    lastResult = SmsSendResult.Failure(
                        ResolveProviderName(),
                        normalizedPhone,
                        "Khong the gui SMS toi provider.",
                        attemptCount: attempt);
                }

                await Task.Delay(
                    TimeSpan.FromMilliseconds(Math.Clamp(_configuration.RetryDelayMilliseconds * attempt, 100, 5000)),
                    cancellationToken);
            }

            return lastResult ?? SmsSendResult.Failure(ResolveProviderName(), normalizedPhone, "Khong the gui SMS.");
        }

        private async Task<SmsSendResult> SendWithESmsAsync(
            string phone,
            string message,
            int attempt,
            CancellationToken cancellationToken)
        {
            var payload = new Dictionary<string, object?>
            {
                ["ApiKey"] = _configuration.ApiKey,
                ["SecretKey"] = _configuration.SecretKey,
                ["Phone"] = phone,
                ["Content"] = message,
                ["SmsType"] = _configuration.SmsType
            };

            if (!string.IsNullOrWhiteSpace(_configuration.BrandName))
            {
                payload["Brandname"] = _configuration.BrandName;
            }

            using var content = new StringContent(
                JsonSerializer.Serialize(payload, SerializerOptions),
                Encoding.UTF8,
                "application/json");

            _logger.LogInformation(
                "Sending SMS via {Provider}. Phone: {Phone}, Length: {Length}, Attempt: {Attempt}",
                ResolveProviderName(),
                MaskPhone(phone),
                message.Length,
                attempt);

            if (_configuration.LogMessageContent)
            {
                _logger.LogDebug("SMS content for {Phone}: {Message}", MaskPhone(phone), message);
            }

            using var response = await _httpClient.PostAsync(_configuration.BaseUrl, content, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);
            var providerResult = ParseProviderResponse(raw);
            var isSuccess = response.IsSuccessStatusCode && IsProviderSuccess(providerResult.ProviderCode);

            if (isSuccess)
            {
                _logger.LogInformation(
                    "SMS sent successfully. Phone: {Phone}, MessageId: {MessageId}, Attempt: {Attempt}",
                    MaskPhone(phone),
                    providerResult.MessageId,
                    attempt);

                return SmsSendResult.Success(
                    ResolveProviderName(),
                    phone,
                    providerResult.MessageId,
                    providerResult.ProviderCode,
                    providerResult.ProviderMessage,
                    (int)response.StatusCode,
                    attempt);
            }

            _logger.LogWarning(
                "SMS provider rejected request. Phone: {Phone}, HttpStatus: {Status}, ProviderCode: {ProviderCode}, ProviderMessage: {ProviderMessage}",
                MaskPhone(phone),
                (int)response.StatusCode,
                providerResult.ProviderCode,
                providerResult.ProviderMessage);

            return SmsSendResult.Failure(
                ResolveProviderName(),
                phone,
                "Provider khong chap nhan yeu cau gui SMS.",
                providerResult.ProviderCode,
                providerResult.ProviderMessage,
                (int)response.StatusCode,
                attempt);
        }

        private static ProviderSmsResult ParseProviderResponse(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return new ProviderSmsResult(null, null, "Provider tra response rong.");
            }

            try
            {
                using var document = JsonDocument.Parse(raw);
                var root = document.RootElement;
                return new ProviderSmsResult(
                    TryGetString(root, "CodeResult") ?? TryGetString(root, "code") ?? TryGetString(root, "Code"),
                    TryGetString(root, "SMSID") ?? TryGetString(root, "SmsId") ?? TryGetString(root, "MessageId"),
                    TryGetString(root, "ErrorMessage") ?? TryGetString(root, "message") ?? TryGetString(root, "Message"));
            }
            catch (JsonException)
            {
                return new ProviderSmsResult(null, null, "Provider tra response khong phai JSON.");
            }
        }

        private static string? NormalizeVietnamPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return null;
            }

            var normalized = phone.Trim()
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace(".", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty);

            if (normalized.StartsWith("+", StringComparison.Ordinal))
            {
                normalized = normalized[1..];
            }

            if (normalized.StartsWith("0", StringComparison.Ordinal) && normalized.Length >= 10)
            {
                normalized = "84" + normalized[1..];
            }

            return PhoneRegex.IsMatch(normalized) ? normalized : null;
        }

        private static bool IsProviderSuccess(string? providerCode)
        {
            if (string.IsNullOrWhiteSpace(providerCode))
            {
                return false;
            }

            return providerCode is "100" or "200" or "0" or "OK" or "Success";
        }

        private static string? TryGetString(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var value)
                ? value.ToString()
                : null;
        }

        private string ResolveProviderName()
            => string.IsNullOrWhiteSpace(_configuration.Provider) ? "ESms" : _configuration.Provider;

        private static string MaskPhone(string phone)
            => phone.Length <= 4 ? "****" : $"{new string('*', Math.Max(0, phone.Length - 4))}{phone[^4..]}";

        private sealed record ProviderSmsResult(
            string? ProviderCode,
            string? MessageId,
            string? ProviderMessage);
    }
}
