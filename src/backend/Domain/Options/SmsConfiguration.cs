namespace Domain.Options;

public sealed class SmsConfiguration
{
    public bool Enabled { get; set; } = false;
    public string Provider { get; set; } = "Mock";
    public string BaseUrl { get; set; } = "https://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/";
    public string ApiKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public int SmsType { get; set; } = 8;
    public int TimeoutSeconds { get; set; } = 15;
    public int MaxRetryAttempts { get; set; } = 3;
    public int RetryDelayMilliseconds { get; set; } = 300;
    public int MaxMessageLength { get; set; } = 500;
    public bool LogMessageContent { get; set; } = false;
    public string OtpTemplate { get; set; } = "Ma OTP Fabu cua ban la {otp}. Ma het han sau 5 phut.";
}
