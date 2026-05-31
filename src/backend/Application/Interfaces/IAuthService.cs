using Application.DTOs.Requests.LoginRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.LoginResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(RefreshRequest request);
        Task LogoutAsync(string? refreshToken, string? accessToken = null);
        Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request);
        Task<ApiResponse<LoginResponse>> RefreshTokenFromCookieAsync(string refreshToken);
        Task<LoginResponse> ExternalLoginAsync(ClaimsPrincipal principal, string provider);
        Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
