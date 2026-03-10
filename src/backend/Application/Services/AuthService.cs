using Application.DTOs.Requests.LoginRequest;
using Application.DTOs.Responses.LoginResponse;
using Application.Interfaces;
using Domain.Configurations;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt; //de hash RT
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration; // de lay jwt setting
        private readonly IValidator<LoginRequest> _validator; //fluentValid
        private readonly IResponseCacheService _responseCache;
        private readonly TimeSpan _refreshTokenExpiry = TimeSpan.FromDays(7); //7 days
        private readonly JwtConfiguration _jwtConfiguration;

        public AuthService(UserManager<User> userManager, IConfiguration configuration, IValidator<LoginRequest> validator,IResponseCacheService responseCache, IOptions<JwtConfiguration> jwtOptions)
        {
            _userManager = userManager;
            _configuration = configuration;
            _validator = validator;
            _responseCache = responseCache;
            _jwtConfiguration = jwtOptions.Value;
        }

        //chain sub-method
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            //validate input dto + fluent
            await ValidateLoginRequestAsync(request);

            //validate user credentials
            var user = await ValidateUserCredentialsAsync(request.Email, request.Password);
            if (user == null) throw new UnauthorizedAccessException("Invalid email or password");

            //generate claims + scope
            var claims = await GenerateClaimsAsync(user);
            var scope = await BuildScope(user); //custom scope dua vao role
            claims.Add(new Claim("scope", scope)); //add scope as claim

            //generate tokens
            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken(); //random string
            var refreshHash = HashToken(refreshToken); //secure hash

            //store refresh in redis
            await StoreRefreshTokenAsync(user.Id, refreshHash, TimeSpan.FromDays(7));

            //response
            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken, // tra cho client luu secure storage
                ExpiresAt = DateTime.Now.AddMinutes(30), //access expiry
                Claims = claims.Select(c => new LoginResponse.ClaimDto { Type = c.Type, Value = c.Value }).ToList()
            };
        }

        //validate request fluent + basic
        private async Task ValidateLoginRequestAsync(LoginRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors); //400 request
        }

        //validate user/password identity
        private async Task<User?> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return null;
            }
            return user;
        }

        //genrate claims - base + role
        private async Task<List<Claim>> GenerateClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email ?? ""),
                new(ClaimTypes.Name, user.Username ?? "")
            };

            //add roles
            var rolesTask = await _userManager.GetRolesAsync(user);
            //var roles = rolesTask.Result;
            //claims.AddRange(rolesTask.Select(role => new Claim(ClaimTypes.Role, role)));
            foreach (var role in rolesTask)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                //permission khop voi policy trong authInstaller
                if (role == "Admin")
                {
                    claims.Add(new Claim("permission", "user.delete"));
                    claims.Add(new Claim("permission", "user.create"));
                }
            }
            return claims;
        }

        //buildScope- define scope cho token, dua vao role - vd read:user write:order
        private async Task<string> BuildScope(User user)
        {
            var roles = await _userManager.GetRolesAsync(user); // Removed `.Result` to fix the issue
            var scopes = new List<string>();

            if (roles.Contains("Admin"))
            {
                scopes.AddRange(new[] { "read:all", "write:all", "delete:all" });
            }
            else if (roles.Contains("User"))
            {
                scopes.AddRange(new[] { "read:user", "write:profile" });
            }
            else
            {
                scopes.Add("read:public"); // default
            }

            return string.Join(" ", scopes);
        }

        //generate access token 
        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            // 1. Tạo Security Key từ chuỗi Key trong cấu hình
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));

            // 2. Tạo thông tin chữ ký (Credentials)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Tạo cấu trúc Token dựa trên các thuộc tính của _jwtConfiguration
            var token = new JwtSecurityToken(
                issuer: _jwtConfiguration.Issuer,
                audience: _jwtConfiguration.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfiguration.ExpiryMinutes), // Không cần GetValue<double> nữa
                signingCredentials: creds
            );

            // 4. Sinh chuỗi Token cuối cùng
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        //generate rt random long-lived string
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes); //random string 86 char
        }

        //hash token sha256 cho secure storage
        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        //store rt trong redis - key: "refresh:{userId}:{hash}", value: expiry -> expiry
        private async Task StoreRefreshTokenAsync(long userId, string refreshHash, TimeSpan expiry)
        {
            var key = $"refresh:{userId}:{refreshHash}";
            var expiryTime = DateTime.UtcNow.Add(expiry).Ticks;// store as long- ticks
            await _responseCache.SetCacheResponseAsync(key, expiryTime, expiry);
        }

        //refresh token: validate tu redis, generate at moi
        public async Task<LoginResponse> RefreshTokenAsync(RefreshRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                throw new UnauthorizedAccessException("Refresh token is required");
            //extract userId tu AT or tu RT - userId luu o 

            //hash token
            var refreshHash = HashToken(request.RefreshToken);
            var key = $"refresh:{request.UserId}:{refreshHash}";//format refresh:{userId}:{hash}
            var storedExpiryTicks = await _responseCache.GetCachedResponseAsync<long>(key);
            if (storedExpiryTicks == 0 || new DateTime(storedExpiryTicks) < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            //xoa refresh token da su dung - one-time use
            await _responseCache.RemoveCacheResponseAsync(key);

            //lay userId tu redis dua tren hash tokn
            //var userIdString = await _responseCache.GetCachedResponseAsync<string>($"refreshToken:{refreshHash}");
            //if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            //{
            //    throw new UnauthorizedAccessException("Invalid refresh token");
            //}

            //lay user tu db
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) throw new UnauthorizedAccessException("User not found.");

            //generate identity moi : claims + scope
            var claims = await GenerateClaimsAsync(user);
            var scope = await BuildScope(user); //custom scope dua vao role
            claims.Add(new Claim("scope", scope)); //add scope as claim
            //generate tokens
            var newAccessToken = GenerateAccessToken(claims);
            var newRefreshToken = GenerateRefreshToken(); //random string
            var newRefreshHash = HashToken(newRefreshToken); //secure hash luu hash rt tro den userId
            //store new refresh in redis, xoa rt cu
            await StoreRefreshTokenAsync(user.Id, newRefreshHash, TimeSpan.FromDays(7));
            
            //response
            return new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken, // tra cho client luu secure storage
                ExpiresAt = DateTime.Now.AddMinutes(30), //access expiry
                Claims = claims.Select(c => new LoginResponse.ClaimDto {
                    Type = c.Type, 
                    Value = c.Value 
                }).ToList()
            };
        }

        public async Task LogoutAsync(LogoutRequest request)
        {
            // Delete all refresh for user (pattern keys – assume _responseCache supports GetKeysAsync)
            var refreshKeys = await _responseCache.GetCachedResponseAsync<List<string>>($"refresh:{request.UserId}:*");  // List<string> keys
            foreach (var key in refreshKeys ?? new List<string>())
            {
                await _responseCache.RemoveCacheResponseAsync(key);
            }

            // Blacklist access token (optional, for immediate invalidate)
            if (!string.IsNullOrEmpty(request.AccessToken))
            {
                // Parse remaining expiry from token (decode JWT without verify)
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(request.AccessToken);
                var remainingExpiry = jsonToken.ValidTo - DateTime.UtcNow;
                if (remainingExpiry > TimeSpan.Zero)
                {
                    await _responseCache.SetCacheResponseAsync($"blacklist:{request.AccessToken}", true, remainingExpiry);
                }
            }
        }
        //verify token - dung trong middleware - extract claims
    }
}
