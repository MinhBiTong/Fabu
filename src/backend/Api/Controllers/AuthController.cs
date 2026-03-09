using Application.DTOs.Requests.LoginRequest;
using Application.DTOs.Responses.LoginResponse;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [EnableRateLimiting("Login")] //Custom plocy cho login - anti spam
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;
        //ham ho tro tao cookie luu refresh token
        public void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,  //chong XSS, js ko doc dc
                Secure = false, //chi gui qua https, CHUYEN THANH TRUE KHI DEPLOY LEN HTTPS
                SameSite = SameSiteMode.Lax, //chong tan cong CSRF, CHUYEN THANH STRICT KHI DEPLOY LEN HTTPS
                Expires = DateTime.UtcNow.AddDays(7)  //tg song cua Refresh
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var respopnse = await _authService.LoginAsync(request);
                return Ok(respopnse); //token, clains in body, react lay token.header
            }
            catch (ValidationException ve)
            {
                var errors = ve.ValidationResult?.MemberNames?.ToList() ?? new List<string> { ve.Message };
                return BadRequest(errors);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshRequest request)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request);
                //SetRefreshTokenCookie(response.RefreshToken); //neu muon luu refresh token trong cookie
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
            //kiem tra userId tu token co khop voi request ko- anti tampering
            var tokenUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (tokenUserId != request.UserId)
            {
                return BadRequest("User ID mismatch");
            }

            await _authService.LogoutAsync(request);
            return NoContent();
        }
    }
}
