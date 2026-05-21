namespace Application.DTOs.Responses.SmsResponse;

public sealed class SmsSendResult
{
    public bool IsSuccess { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? MessageId { get; set; }
    public string? ProviderCode { get; set; }
    public string? ProviderMessage { get; set; }
    public int? HttpStatusCode { get; set; }
    public int AttemptCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;

    public static SmsSendResult Success(
        string provider,
        string phone,
        string? messageId,
        string? providerCode,
        string? providerMessage,
        int? httpStatusCode,
        int attemptCount)
        => new()
        {
            IsSuccess = true,
            Provider = provider,
            Phone = phone,
            MessageId = messageId,
            ProviderCode = providerCode,
            ProviderMessage = providerMessage,
            HttpStatusCode = httpStatusCode,
            AttemptCount = attemptCount
        };

    public static SmsSendResult Failure(
        string provider,
        string phone,
        string errorMessage,
        string? providerCode = null,
        string? providerMessage = null,
        int? httpStatusCode = null,
        int attemptCount = 0)
        => new()
        {
            IsSuccess = false,
            Provider = provider,
            Phone = phone,
            ErrorMessage = errorMessage,
            ProviderCode = providerCode,
            ProviderMessage = providerMessage,
            HttpStatusCode = httpStatusCode,
            AttemptCount = attemptCount
        };
}
