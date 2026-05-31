using Application.DTOs.Requests;
using Application.DTOs.Requests.LoginRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.LoginResponse;
using Application.Interfaces;
using Azure;
using Domain.Options;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    //[EnableRateLimiting("Login")]
    public class AuthController : ControllerBase
    {
        private readonly IValidator<LoginRequest> _validator;
        private readonly IAuthService _authService;
        private readonly IResponseCacheService _responseCacheService;
        private readonly AuthSecurityConfiguration _authSecurity;
        private ILogger<AuthController> _logger;
        public AuthController(
            IValidator<LoginRequest> validator,
            IAuthService authService,
            IResponseCacheService responseCacheService,
            IOptions<AuthSecurityConfiguration> authSecurityOptions,
            ILogger<AuthController> logger)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _responseCacheService = responseCacheService;
            _authSecurity = authSecurityOptions.Value;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                if (result != null && result is RegisterResponse registerResponse)
                {
                    return Ok(new { message = "Register successfully!", data = registerResponse });
                }
                return BadRequest(ApiResponse<LoginResponse>.Fail(400, "Register failed! Try again please!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<ApiResponse<VerifyOtpResponse>>> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            // Gọi sang AuthService để xử lý logic
            var result = await _authService.VerifyOtpAsync(request);

            // Trả về kết quả cho Client
            return Ok(ApiResponse<VerifyOtpResponse>.Success(result, "Validate OTP successfully."));
        }

        [HttpGet("test-redis")]
        public async Task<IActionResult> TestRedis()
        {
            await _responseCacheService.SetRawStringAsync("test_key", "Hello Redis", TimeSpan.FromMinutes(5));
            var value = await _responseCacheService.GetRawStringAsync("test_key");
            return Ok(new { SavedValue = value });
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(ApiResponse<LoginResponse>.Fail(400, "Invalid input"));

            try
            {
                var response = await _authService.LoginAsync(request);
                SetAuthCookies(response);
                return Ok(ApiResponse<LoginResponse>.Success(response, "Login successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<LoginResponse>.Fail(401, "Email or password invalid"));
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [DisableRateLimiting]
        [HttpGet("signin-google")]
        public IActionResult SignInGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback))
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [DisableRateLimiting]
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
                return BadRequest(new { Message = "Login Google failed!" });

            try
            {
                var response = await _authService.ExternalLoginAsync(authenticateResult.Principal, "google");
                SetAuthCookies(response);
                return Ok(ApiResponse<LoginResponse>.Success(response, "Google login successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<LoginResponse>.Fail(401, "Google account is not linked to an active Fabu user"));
            }
        }

        [DisableRateLimiting]
        [HttpGet("signin-github")]
        public IActionResult SignInGitHub()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GitHubCallback))
            };
            return Challenge(properties, "GitHub");
        }

        [DisableRateLimiting]
        [HttpGet("github-callback")]
        public async Task<IActionResult> GitHubCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("GitHub");

            if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
                return BadRequest(new { Message = "Login GitHub failed!" });

            try
            {
                var response = await _authService.ExternalLoginAsync(authenticateResult.Principal, "github");
                SetAuthCookies(response);
                return Ok(ApiResponse<LoginResponse>.Success(response, "GitHub login successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<LoginResponse>.Fail(401, "GitHub account is not linked to an active Fabu user"));
            }
        }

        [DisableRateLimiting]
        [HttpGet("signin-oidc")]
        public IActionResult SignInOidc([FromQuery] string? returnUrl = null)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(OidcCallback), new { returnUrl })
            };

            return Challenge(properties, "oidc");
        }

        [DisableRateLimiting]
        [HttpGet("oidc-callback")]
        public async Task<IActionResult> OidcCallback([FromQuery] string? returnUrl = null)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Cookies");
            if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
            {
                return Unauthorized(ApiResponse<LoginResponse>.Fail(401, "OIDC login failed"));
            }

            try
            {
                var response = await _authService.ExternalLoginAsync(authenticateResult.Principal, "oidc");
                SetAuthCookies(response);
                return Ok(ApiResponse<LoginResponse>.Success(response, "OIDC login successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<LoginResponse>.Fail(401, "OIDC account is not linked to an active Fabu user"));
            }
        }

        [HttpPost("refresh-token")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken()
        {
            try
            {
                // Backend đọc Refresh Token từ HttpOnly Cookie
                var refreshToken = Request.Cookies[_authSecurity.RefreshTokenCookieName]
                    ?? Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(ApiResponse<LoginResponse>.Fail(401, "Refresh token not exists"));

                // Gọi service với refreshToken từ cookie
                var response = await _authService.RefreshTokenFromCookieAsync(refreshToken);

                // Nếu backend trả về Refresh Token mới → set lại cookie
                if (!string.IsNullOrEmpty(response.Data?.RefreshToken))
                {
                    SetAuthCookies(response.Data);
                }

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<LoginResponse>.Fail(401, "Refresh token invalid or expired"));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Đọc Refresh Token từ HttpOnly Cookie
                var refreshToken = Request.Cookies[_authSecurity.RefreshTokenCookieName]
                    ?? Request.Cookies["refreshToken"];

                // Đọc Access Token từ Header (để blacklist)
                var accessToken = Request.Headers["Authorization"]
                    .ToString()
                    .Replace("Bearer ", "")
                    .Trim();
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    accessToken = Request.Cookies[_authSecurity.AccessTokenCookieName] ?? string.Empty;
                }

                // Gọi Service với dữ liệu từ cookie và header
                await _authService.LogoutAsync(refreshToken, accessToken);

                ClearAuthCookies();

                return Ok(ApiResponse<object>.Success(null, "Logout Successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                ClearAuthCookies();
                return StatusCode(500, ApiResponse<object>.Fail(500, "Logout Failed"));
            }
        }

        [HttpPost("forgot-password")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var response = await _authService.ForgotPasswordAsync(request);
            return StatusCode(response.Code, response);
        }

        [HttpPost("reset-password")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var response = await _authService.ResetPasswordAsync(request);
            return StatusCode(response.Code, response);
        }

        private void SetAuthCookies(LoginResponse response)
        {
            SetAccessTokenCookie(response.AccessToken);
            SetRefreshTokenCookie(response.RefreshToken);
        }

        private void SetAccessTokenCookie(string accessToken)
        {
            var cookieOptions = BuildCookieOptions(DateTimeOffset.UtcNow.AddMinutes(_authSecurity.AccessTokenMinutes));
            Response.Cookies.Append(_authSecurity.AccessTokenCookieName, accessToken, cookieOptions);
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = BuildCookieOptions(DateTimeOffset.UtcNow.AddDays(_authSecurity.RefreshTokenDays));
            Response.Cookies.Append(_authSecurity.RefreshTokenCookieName, refreshToken, cookieOptions);
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private void ClearAuthCookies()
        {
            Response.Cookies.Delete(_authSecurity.AccessTokenCookieName, BuildCookieOptions(DateTimeOffset.UtcNow.AddDays(-1)));
            Response.Cookies.Delete(_authSecurity.RefreshTokenCookieName, BuildCookieOptions(DateTimeOffset.UtcNow.AddDays(-1)));
            Response.Cookies.Delete("refreshToken", BuildCookieOptions(DateTimeOffset.UtcNow.AddDays(-1)));
        }

        private CookieOptions BuildCookieOptions(DateTimeOffset expires)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = _authSecurity.CookieSecure,
                SameSite = ParseSameSite(_authSecurity.CookieSameSite),
                Expires = expires
            };
        }

        private static SameSiteMode ParseSameSite(string value)
        {
            return Enum.TryParse<SameSiteMode>(value, ignoreCase: true, out var sameSite)
                ? sameSite
                : SameSiteMode.Lax;
        }
    }
}
