using Application.DTOs.Requests.LoginRequest;
using Application.DTOs.Responses.LoginResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(RefreshRequest request);
        Task LogoutAsync(LogoutRequest request);
    }
}
