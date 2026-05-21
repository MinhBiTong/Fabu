using System.ComponentModel.DataAnnotations;

namespace Domain.Options
{
    public class PayPalConfiguration
    {
        public bool Enabled { get; set; }
        public string BaseUrl { get; set; } = "https://api-m.sandbox.paypal.com";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
    }
}
