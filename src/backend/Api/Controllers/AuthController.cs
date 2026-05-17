using Application.DTOs.Requests;
using Application.DTOs.Requests.LoginRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.LoginResponse;
using Application.Interfaces;
using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        private ILogger<AuthController> _logger;
        public AuthController(IValidator<LoginRequest> validator, IAuthService authService, IResponseCacheService responseCacheService, ILogger<AuthController> logger)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _responseCacheService = responseCacheService;
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
        //[EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(ApiResponse<LoginResponse>.Fail(400, "Invalid input"));

            try
            {
                var response = await _authService.LoginAsync(request);
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

            if (!authenticateResult.Succeeded)
                return BadRequest(new { Message = "Login Google failed!" });

            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email))
                return BadRequest(new { Message = "Can't get email from Google." });

            return Ok(new
            {
                Message = "Get Google's info successfully!",
                Email = email,
                Name = name,
                GoogleId = googleId
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken()
        {
            try
            {
                // Backend đọc Refresh Token từ HttpOnly Cookie
                var refreshToken = Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(ApiResponse<LoginResponse>.Fail(401, "Refresh token not exists"));

                // Gọi service với refreshToken từ cookie
                var response = await _authService.RefreshTokenFromCookieAsync(refreshToken);

                // Nếu backend trả về Refresh Token mới → set lại cookie
                if (!string.IsNullOrEmpty(response.RefreshToken))
                {
                    SetRefreshTokenCookie(response.RefreshToken);
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
                var refreshToken = Request.Cookies["refreshToken"];

                // Đọc Access Token từ Header (để blacklist)
                var accessToken = Request.Headers["Authorization"]
                    .ToString()
                    .Replace("Bearer ", "")
                    .Trim();

                // Gọi Service với dữ liệu từ cookie và header
                await _authService.LogoutAsync(refreshToken, accessToken);

                // Xóa cookie refreshToken
                Response.Cookies.Delete("refreshToken");

                return Ok(ApiResponse<object>.Success(null, "Logout Successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                Response.Cookies.Delete("refreshToken"); // vẫn xóa cookie dù có lỗi
                return StatusCode(500, ApiResponse<object>.Fail(500, "Logout Failed"));
            }
        }

        // Private helper method - không cần HttpMethod vì không phải action public
        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // true khi deploy HTTPS
                SameSite = SameSiteMode.Lax, // Strict khi HTTPS
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}