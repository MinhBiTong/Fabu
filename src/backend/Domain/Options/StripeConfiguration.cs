namespace Domain.Options
{
    public class StripeConfiguration
    {
        public bool Enabled { get; set; }
        public string SecretKey { get; set; } = string.Empty;
        public string CheckoutSessionUrl { get; set; } = "https://api.stripe.com/v1/checkout/sessions";
        public string SuccessUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string Currency { get; set; } = "usd";
    }
}
