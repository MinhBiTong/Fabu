using Application.DTOs.Requests;
using Application.DTOs.Requests.LoginRequest;
using Application.DTOs.Responses.LoginResponse;
using Application.Interfaces;
using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

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

        public AuthController(IValidator<LoginRequest> validator, IAuthService authService)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                if (result)
                {
                    return Ok(new { message = "Register successfully!" });
                }
                return BadRequest(ApiResponse<LoginResponse>.Fail(400, "Register failed! Try again please!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
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
        public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshRequest request) 
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request);
                // Nếu muốn lưu refresh token vào cookie (secure hơn lưu ở localStorage)
                SetRefreshTokenCookie(response.RefreshToken);
                return Ok(ApiResponse<LoginResponse>.Success(response, "Refresh token successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<LoginResponse>.Fail(401, "Refresh token invalid or expired"));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request) 
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int tokenUserId))
                return BadRequest(ApiResponse<object>.Fail(400, "Token invalid"));

            if (tokenUserId != request.UserId)
                return BadRequest(ApiResponse<object>.Fail(400, "User ID mismatch"));

            await _authService.LogoutAsync(request);
            // Xóa cookie refresh token nếu dùng cookie
            Response.Cookies.Delete("refreshToken");
            return Ok(ApiResponse<object>.Success(null, "Logout successfully"));
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