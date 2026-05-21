using System.ComponentModel.DataAnnotations;

namespace Domain.Options
{
    public class AuthSecurityConfiguration
    {
        public string AccessTokenCookieName { get; set; } = "fabu_at";
        public string RefreshTokenCookieName { get; set; } = "fabu_rt";

        [Range(1, 1440)]
        public int AccessTokenMinutes { get; set; } = 10;

        [Range(1, 365)]
        public int RefreshTokenDays { get; set; } = 30;

        [Range(1, 60)]
        public int OtpTtlMinutes { get; set; } = 5;

        [Range(1, 60)]
        public int PasswordResetTtlMinutes { get; set; } = 10;

        public bool CookieSecure { get; set; } = true;
        public string CookieSameSite { get; set; } = "Lax";
    }
}
