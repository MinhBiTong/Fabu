using Application.DTOs.Requests;
using Application.DTOs.Requests.LoginRequest;
using Application.DTOs.Responses.LoginResponse;
using Application.Interfaces;
using FluentValidation;
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

        // Constructor duy nhất
        public AuthController(IValidator<LoginRequest> validator, IAuthService authService)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials" });
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
                return BadRequest(new { Message = "Đăng nhập Google thất bại!" });

            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email))
                return BadRequest(new { Message = "Không lấy được email từ Google." });

            return Ok(new
            {
                Message = "Đã lấy được thông tin từ Google!",
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
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request) 
        {
            var tokenUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (tokenUserId != request.UserId)
            {
                return BadRequest("User ID mismatch");
            }

            await _authService.LogoutAsync(request);
            // Xóa cookie refresh token nếu dùng cookie
            Response.Cookies.Delete("refreshToken");
            return NoContent();
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