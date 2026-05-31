using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Security
{
    public static class AuthCacheKeys
    {
        public const string Namespace = "v1:auth";

        public static string RefreshToken(string tokenHash) => $"{Namespace}:refresh:{tokenHash}";
        public static string UserRefreshToken(long userId, string tokenHash) => $"{Namespace}:user-refresh:{userId}:{tokenHash}";
        public static string UserRefreshTokenGroup(long userId) => $"{Namespace}:user-refresh:{userId}";
        public static string Session(string sessionId) => $"{Namespace}:session:{sessionId}";
        public static string OtpVerify(long userId) => $"{Namespace}:otp:verify:{userId}";
        public static string OtpLimit(long userId) => $"{Namespace}:rate:otp:{userId}";
        public static string ForgotPasswordLimit(string emailHash) => $"{Namespace}:rate:forgot-password:{emailHash}";
        public static string PasswordReset(string emailHash, string tokenHash) => $"{Namespace}:reset-password:{emailHash}:{tokenHash}";
        public static string AccessTokenBlacklist(string jti) => $"{Namespace}:blacklist:access:{jti}";

        public static string Sha256(string value)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToHexString(sha256.ComputeHash(bytes)).ToLowerInvariant();
        }

        public static string NewSecureToken(int byteLength = 64)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(byteLength);
            return Base64UrlEncoder.Encode(randomBytes);
        }
    }
}
