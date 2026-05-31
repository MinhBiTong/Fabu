using Application.DTOs.Requests.LoginRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.LoginResponse;
using Application.Interfaces;
using Application.Common.Security;
using Azure.Core;
using Domain.Abstractions;
using Domain.Configurations;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Options;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt; 
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration; // de lay jwt setting
        private readonly IResponseCacheService? _responseCache;
        private readonly TimeSpan _refreshTokenExpiry;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly AuthSecurityConfiguration _authSecurity;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<LoginRequest> _validator;
        private readonly ILogger<AuthService> _logger;
        private readonly ICustomerService _customerService;
        private readonly IEmailService? _emailService;

        public AuthService(
            IConfiguration configuration,
            IValidator<LoginRequest> validator,
            IOptions<JwtConfiguration> jwtOptions,
            IOptions<AuthSecurityConfiguration> authSecurityOptions,
            IUnitOfWork unitOfWork,
            ILogger<AuthService> logger,
            ICustomerService customerService,
            IEmailService? emailService = null,
            IResponseCacheService? responseCache = null)
        {
            _configuration = configuration;
            _responseCache = responseCache;
            _jwtConfiguration = jwtOptions.Value;
            _authSecurity = authSecurityOptions.Value;
            _refreshTokenExpiry = TimeSpan.FromDays(Math.Max(1, _authSecurity.RefreshTokenDays));
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
            _customerService = customerService;
            _emailService = emailService;
        }

        private TimeSpan AccessTokenLifetime =>
            TimeSpan.FromMinutes(Math.Max(1, _authSecurity.AccessTokenMinutes));

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {

            //check email ton tai
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (existingUser != null) throw new AppException(ErrorCode.EMAIL_ALREADY_EXISTS, "Email already exists");
            var existingCustomer = await _unitOfWork.Customers.GetByMobileNumberAsync(request.PhoneNumber);
            if (existingCustomer != null)
                throw new AppException(ErrorCode.PHONE_ALREADY_EXISTS, "Phone number already exists");
            // get Role "Customer" from database
            var customerRole = await _unitOfWork.Roles.GetByNameAsync("Customer");
            if (customerRole == null) throw new Exception("The system not config Role Customer yet");

            //2. hash pw
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            //3. tao user moi
            var newUser = new User
            {
                Username = request.Username,
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                CreatedDate = DateTime.UtcNow,
                IsActive = false,
                IsDeleted = false,
                UserRoles = new List<UserRole>
                {
                    new UserRole 
                    { 
                        RoleId = customerRole.Id                    
                    }
                }
            };

            _logger.LogInformation("User registered successfully. UserId: {UserId}, Email: {Email}", newUser.Id, newUser.Email);
            //4. luu vao db thong qua repo va uow
            await _unitOfWork.Users.AddAsync(newUser);
            var result = await _unitOfWork.CommitAsync();

            await SendOtpAsync(newUser.Id, newUser.PhoneNumber);

            _logger.LogInformation("User created (inactive). UserId: {UserId}", newUser.Id);

            return new RegisterResponse
            {
                UserId = newUser.Id,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                Message = "Registration successful. Please check the OTP sent to your phone number.",
                RequiresOtpVerification = true
            };
        }

        public async Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                throw new AppException(ErrorCode.USER_NOT_EXISTED);

            if (user.IsActive)
                return new VerifyOtpResponse { Success = true, Message = "The account was previously activated." };

            if (_responseCache == null)
                throw new AppException(ErrorCode.OTP_SERVICE_UNAVAILABLE, "OTP cache service is not available.");

            var otpKey = AuthCacheKeys.OtpVerify(user.Id);
            var cachedOtp = await _responseCache.GetCachedResponseAsync<string>(otpKey);
            if (string.IsNullOrWhiteSpace(cachedOtp))
                throw new AppException(ErrorCode.OTP_EXPIRED, "OTP expired or not found.");

            if (!string.Equals(cachedOtp, request.Otp, StringComparison.Ordinal))
                throw new AppException(ErrorCode.INVALID_OTP, "OTP is invalid.");

            // Gọi business tạo Customer + Account
            var customerResult = await _customerService.VerifyOtpAndCreateCustomerAsync(user.Id, request.Otp);

            //if (!customerResult.IsSuccess)
            //    throw new AppException(ErrorCode.CUSTOMER_CREATION_FAILED, customerResult.Message);

            // Cập nhật User.IsActive = true
            user.IsActive = true;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} verified OTP successfully and customer account created.", user.Id);

            //xoa OTP trong Redis sau khi verify thanh cong - clean up
            if (_responseCache != null)
            {
                await _responseCache.RemoveCacheResponseAsync(otpKey);
                await _responseCache.RemoveCacheResponseAsync($"otp:verify:{user.Id}");
            }

            return new VerifyOtpResponse
            {
                Success = true,
                Message = "OTP verification successful. Account has been activated.",
                CustomerId = customerResult.Data?.Id
            };
        }

        // ====================== Helper Methods ======================
        private async Task SendOtpAsync(long userId, string phoneNumber)
        {
            if (_responseCache == null)
                throw new AppException(ErrorCode.OTP_SERVICE_UNAVAILABLE, "OTP cache service is not available.");

            var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var ttl = TimeSpan.FromMinutes(Math.Max(1, _authSecurity.OtpTtlMinutes));
            var key = AuthCacheKeys.OtpVerify(userId);
            await _responseCache.SetCacheResponseAsync(key, otp, ttl);

            _logger.LogInformation("OTP issued for user {UserId}, phone {Phone}. TTL: {TtlMinutes} minutes",
                userId, phoneNumber, ttl.TotalMinutes);
        }

        public async Task<ApiResponse<bool>> ResendOtpAsync(ResendOtpRequest request)
        {
            // 1. Tìm User theo số điện thoại hoặc UserId
            var user = await _unitOfWork.Users.GetByMobileNumberAsync(request.PhoneNumber);
            if (user == null) return ApiResponse<bool>.Fail(404, "Phone number hasn't been register");

            // 2. Nếu tài khoản đã active rồi thì không gửi lại làm gì
            if (user.IsActive) return ApiResponse<bool>.Fail(400, "The account was previously activated.");

            if (_responseCache == null)
                throw new AppException(ErrorCode.OTP_SERVICE_UNAVAILABLE, "OTP cache service is not available.");

            // 3. (Optional) Check Rate Limit - Tránh spam SMS tốn tiền
            var rateLimitKey = AuthCacheKeys.OtpLimit(user.Id);
            var isWaiting = await _responseCache.GetCachedResponseAsync<string>(rateLimitKey);
            if (!string.IsNullOrEmpty(isWaiting))
                return ApiResponse<bool>.Fail(429, "Please wait 60s before request new OTP.");

            // 4. Gửi OTP mới
            await SendOtpAsync(user.Id, user.PhoneNumber);

            // 5. Đặt rate limit 60 giây
            await _responseCache.SetCacheResponseAsync(rateLimitKey, "true", TimeSpan.FromSeconds(60));

            return ApiResponse<bool>.Success(true, "New OTP sent successfully.");
        }

        //chain sub-method
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            //validate input dto + fluent
            await ValidateLoginRequestAsync(request);

            //validate user credentials
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Invalid email or password");
            if (!user.IsActive) throw new UnauthorizedAccessException("User is inactive or OTP has not been verified.");

            //generate claims + scope
            var sessionId = Guid.NewGuid().ToString("N");
            var claims = await GenerateClaimsAsync(user);
            claims.Add(new Claim("sid", sessionId));
            var scope = await BuildScope(user); //custom scope dua vao role
            claims.Add(new Claim("scope", scope)); //add scope as claim

            //generate tokens
            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken(); //random string

            //store refresh in redis
            await StoreRefreshTokenAsync(user.Id, refreshToken, sessionId, "password", _refreshTokenExpiry);

            //response
            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken, // tra cho client luu secure storage
                ExpiresAt = DateTime.UtcNow.Add(AccessTokenLifetime), //access expiry
                Claims = claims.Select(c => new LoginResponse.ClaimDto { Type = c.Type, Value = c.Value }).ToList()
            };
        }

        private bool VerifyPassword(string password, string passwordHash) =>  BCrypt.Net.BCrypt.Verify(password, passwordHash);

        //validate request fluent + basic
        private async Task ValidateLoginRequestAsync(LoginRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid) throw new AppException(ErrorCode.UNAUTHENTICATED); //400 request
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

            var roles = user.UserRoles?
                .Select(ur => ur.Role)
                .Where(role => role != null && !string.IsNullOrWhiteSpace(role.Name))
                .GroupBy(role => role!.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First()!)
                .ToList() ?? new List<Role>();

            foreach (var roleEntity in roles)
            {
                var role = roleEntity.Name.Trim();
                claims.Add(new Claim(ClaimTypes.Role, role));

                var permissions = roleEntity.RolePermissions?
                    .Select(rp => rp.Permission?.Name)
                    .Where(permission => !string.IsNullOrWhiteSpace(permission))
                    .Select(permission => permission!.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList() ?? new List<string>();

                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("permission", permission));
                }

                if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    claims.Add(new Claim("permission", "user.delete"));
                    claims.Add(new Claim("permission", "user.create"));
                    claims.Add(new Claim("permission", "post.edit"));
                    claims.Add(new Claim("permission", "payment.manage"));
                    claims.Add(new Claim("permission", "system.audit.read"));
                }
            }
            return claims;
        }

        //buildScope- define scope cho token, dua vao role - vd read:user write:order
        private async Task<string> BuildScope(User user)
        {
            var roles = user.UserRoles?
                .Select(ur => ur.Role?.Name)
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => role!)
                .ToList() ?? new List<string>();
            var scopes = new List<string>();

            if (roles.Contains("Admin", StringComparer.OrdinalIgnoreCase))
            {
                scopes.AddRange(new[] { "read:all", "write:all", "delete:all" });
            }
            else if (roles.Contains("Customer", StringComparer.OrdinalIgnoreCase))
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
            var claimList = claims.ToList();
            if (!claimList.Any(claim => claim.Type == JwtRegisteredClaimNames.Jti))
            {
                claimList.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
            }

            if (!claimList.Any(claim => claim.Type == JwtRegisteredClaimNames.Iat))
            {
                claimList.Add(new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64));
            }

            // 1. Tạo Security Key từ chuỗi Key trong cấu hình
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));

            // 2. Tạo thông tin chữ ký (Credentials)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Tạo cấu trúc Token dựa trên các thuộc tính của _jwtConfiguration
            var token = new JwtSecurityToken(
                issuer: _jwtConfiguration.Issuer,
                audience: _jwtConfiguration.Audience,
                claims: claimList,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(AccessTokenLifetime),
                signingCredentials: creds
            );

            // 4. Sinh chuỗi Token cuối cùng
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //generate rt random long-lived string
        private string GenerateRefreshToken()
        {
            return AuthCacheKeys.NewSecureToken();
        }

        //hash token sha256 cho secure storage
        private string HashToken(string token)
        {
            return AuthCacheKeys.Sha256(token);
        }

        private async Task StoreRefreshTokenAsync(long userId, string refreshToken, string sessionId, string provider, TimeSpan expiry)
        {
            if (_responseCache == null)
                throw new InvalidOperationException("Response cache service is not initialized.");

            var refreshHash = HashToken(refreshToken);
            var issuedAt = DateTimeOffset.UtcNow;
            var entry = new RefreshTokenCacheEntry
            {
                UserId = userId,
                TokenHash = refreshHash,
                SessionId = sessionId,
                Provider = provider,
                IssuedAt = issuedAt,
                ExpiresAt = issuedAt.Add(expiry)
            };

            var tokenKey = AuthCacheKeys.RefreshToken(refreshHash);
            var userTokenKey = AuthCacheKeys.UserRefreshToken(userId, refreshHash);
            var sessionKey = AuthCacheKeys.Session(sessionId);
            var userGroupKey = AuthCacheKeys.UserRefreshTokenGroup(userId);

            await _responseCache.SetCacheResponseByGroupAsync(tokenKey, entry, absoluteExpiry: expiry);
            await _responseCache.SetCacheResponseByGroupAsync(userTokenKey, entry, absoluteExpiry: expiry);
            await _responseCache.SetCacheResponseByGroupAsync(sessionKey, entry, absoluteExpiry: expiry);
            await _responseCache.AddToGroupAsync(userGroupKey, tokenKey);
            await _responseCache.AddToGroupAsync(userGroupKey, userTokenKey);
            await _responseCache.AddToGroupAsync(userGroupKey, sessionKey);

            _logger.LogInformation("Stored refresh token in Redis. UserId: {UserId}, SessionId: {SessionId}, TTLDays: {TtlDays}",
                userId, sessionId, expiry.TotalDays);
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                throw new UnauthorizedAccessException("Refresh token is required");

            return await RefreshTokenCoreAsync(request.RefreshToken, request.UserId);
        }

        public async Task<ApiResponse<LoginResponse>> RefreshTokenFromCookieAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is required");

            var response = await RefreshTokenCoreAsync(refreshToken);
            return ApiResponse<LoginResponse>.Success(response);
        }

        private async Task<LoginResponse> RefreshTokenCoreAsync(string refreshToken, long? expectedUserId = null)
        {
            if (_responseCache == null)
                throw new InvalidOperationException("Response cache service is not initialized.");

            var refreshHash = HashToken(refreshToken);
            var tokenKey = AuthCacheKeys.RefreshToken(refreshHash);
            var entry = await _responseCache.GetCachedResponseAsync<RefreshTokenCacheEntry>(tokenKey);
            if (entry == null || entry.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _logger.LogWarning("Refresh token not found or expired. Key: {Key}", tokenKey);
                throw new UnauthorizedAccessException("Refresh token invalid or expired");
            }

            if (expectedUserId.HasValue && expectedUserId.Value > 0 && entry.UserId != expectedUserId.Value)
            {
                _logger.LogWarning("Refresh token user mismatch. Expected: {ExpectedUserId}, Actual: {ActualUserId}",
                    expectedUserId.Value, entry.UserId);
                throw new UnauthorizedAccessException("Refresh token invalid");
            }

            await RemoveRefreshTokenEntryAsync(entry);

            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(entry.UserId);
            if (user == null || !user.IsActive)
                throw new AppException(ErrorCode.USER_NOT_EXISTED, "User not found or inactive");

            var claims = await GenerateClaimsAsync(user);
            claims.Add(new Claim("sid", entry.SessionId));
            var scope = await BuildScope(user);
            claims.Add(new Claim("scope", scope));

            var newAccessToken = GenerateAccessToken(claims);
            var newRefreshToken = GenerateRefreshToken();
            await StoreRefreshTokenAsync(user.Id, newRefreshToken, entry.SessionId, entry.Provider, _refreshTokenExpiry);

            _logger.LogInformation("Refresh token rotated successfully. UserId: {UserId}, SessionId: {SessionId}",
                user.Id, entry.SessionId);

            return new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.Add(AccessTokenLifetime),
                Claims = claims.Select(c => new LoginResponse.ClaimDto
                {
                    Type = c.Type,
                    Value = c.Value
                }).ToList()
            };
        }

        private async Task RemoveRefreshTokenEntryAsync(RefreshTokenCacheEntry entry)
        {
            if (_responseCache == null) return;

            await _responseCache.RemoveCacheResponseAsync(AuthCacheKeys.RefreshToken(entry.TokenHash));
            await _responseCache.RemoveCacheResponseAsync(AuthCacheKeys.UserRefreshToken(entry.UserId, entry.TokenHash));
            await _responseCache.RemoveCacheResponseAsync(AuthCacheKeys.Session(entry.SessionId));
        }

        public async Task<LoginResponse> ExternalLoginAsync(ClaimsPrincipal principal, string provider)
        {
            var email = principal.FindFirst(ClaimTypes.Email)?.Value
                ?? principal.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(email))
                throw new UnauthorizedAccessException("External provider did not return an email claim.");

            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("External account is not linked to an active Fabu user.");

            var sessionId = Guid.NewGuid().ToString("N");
            var claims = await GenerateClaimsAsync(user);
            claims.Add(new Claim("sid", sessionId));
            claims.Add(new Claim("idp", provider));
            claims.Add(new Claim("amr", "external"));

            var scope = await BuildScope(user);
            claims.Add(new Claim("scope", scope));

            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken();
            await StoreRefreshTokenAsync(user.Id, refreshToken, sessionId, provider, _refreshTokenExpiry);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.Add(AccessTokenLifetime),
                Claims = claims.Select(c => new LoginResponse.ClaimDto
                {
                    Type = c.Type,
                    Value = c.Value
                }).ToList()
            };
        }

        //refresh token: validate tu redis, generate at moi
        //public async Task<LoginResponse> RefreshTokenAsync(RefreshRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.RefreshToken))
        //        throw new UnauthorizedAccessException("Refresh token is required");
        //    //extract userId tu AT or tu RT - userId luu o 

        //    //hash token
        //    var refreshHash = HashToken(request.RefreshToken);
        //    var key = $"refresh:{request.UserId}:{refreshHash}";//format refresh:{userId}:{hash}

        //    _logger.LogInformation("Checking Redis Key: {Key}", key);

        //    var storedExpiryTicks = await _responseCache.GetCachedResponseAsync<long?>(key);

        //    _logger.LogInformation("Stored Ticks in Redis: {Ticks}", storedExpiryTicks);

        //    if (storedExpiryTicks == null || storedExpiryTicks < DateTime.UtcNow.Ticks)
        //    {
        //        _logger.LogWarning("Refresh Token validation failed for Key: {Key}", key);
        //        throw new AppException(ErrorCode.UNAUTHENTICATED, "Invalid or expired refresh token");
        //    }

        //    //xoa refresh token da su dung - one-time use
        //    await _responseCache.RemoveCacheResponseAsync(key);

        //    //lay user tu db
        //    var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        //    if (user == null) throw new AppException(ErrorCode.USER_NOT_EXISTED, "User not found");

        //    //generate identity moi : claims + scope
        //    var claims = await GenerateClaimsAsync(user);
        //    var scope = await BuildScope(user); //custom scope dua vao role
        //    claims.Add(new Claim("scope", scope)); //add scope as claim
        //    //generate tokens
        //    var newAccessToken = GenerateAccessToken(claims);
        //    var newRefreshToken = GenerateRefreshToken(); //random string
        //    var newRefreshHash = HashToken(newRefreshToken); //secure hash luu hash rt tro den userId
        //    //store new refresh in redis, xoa rt cu
        //    await StoreRefreshTokenAsync(user.Id, newRefreshHash, TimeSpan.FromDays(7));

        //    //response
        //    return new LoginResponse
        //    {
        //        AccessToken = newAccessToken,
        //        RefreshToken = newRefreshToken, // tra cho client luu secure storage
        //        ExpiresAt = DateTime.Now.AddMinutes(_jwtConfiguration.ExpiryMinutes), //access expiry
        //        Claims = claims.Select(c => new LoginResponse.ClaimDto {
        //            Type = c.Type, 
        //            Value = c.Value 
        //        }).ToList()
        //    };
        //}

        public async Task LogoutAsync(string? refreshToken, string? accessToken = null)
        {
            if (_responseCache == null)
            {
                _logger.LogWarning("Cache service not available during logout");
                return;
            }
            try
            {
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var refreshHash = HashToken(refreshToken);
                    var entry = await _responseCache.GetCachedResponseAsync<RefreshTokenCacheEntry>(
                        AuthCacheKeys.RefreshToken(refreshHash));

                    if (entry != null)
                    {
                        await RemoveRefreshTokenEntryAsync(entry);
                        _logger.LogInformation("Removed refresh token session during logout. UserId: {UserId}, SessionId: {SessionId}",
                            entry.UserId, entry.SessionId);
                    }
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(accessToken))
                    {
                        var jwt = handler.ReadJwtToken(accessToken);
                        var remaining = jwt.ValidTo - DateTime.UtcNow;
                        var jti = jwt.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value
                            ?? HashToken(accessToken);

                        if (remaining > TimeSpan.Zero)
                        {
                            await _responseCache.SetCacheResponseByGroupAsync(
                                AuthCacheKeys.AccessTokenBlacklist(jti),
                                true,
                                absoluteExpiry: remaining);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout process");
                // Không throw exception ở logout để tránh làm hỏng UX
            }
        }

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (_responseCache == null)
                throw new InvalidOperationException("Response cache service is not initialized.");

            var normalizedEmail = NormalizeEmail(request.Email);
            var emailHash = AuthCacheKeys.Sha256(normalizedEmail);
            var rateLimitKey = AuthCacheKeys.ForgotPasswordLimit(emailHash);

            var rateLimited = await _responseCache.GetCachedResponseAsync<string>(rateLimitKey);
            if (!string.IsNullOrEmpty(rateLimited))
                return ApiResponse<bool>.Fail(429, "Please wait before requesting another password reset token.");

            await _responseCache.SetCacheResponseAsync(rateLimitKey, "1", TimeSpan.FromSeconds(60));

            var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);
            if (user == null || user.IsDeleted)
            {
                _logger.LogInformation("Password reset requested for non-existing email hash {EmailHash}", emailHash);
                return ApiResponse<bool>.Success(true, "If the email exists, a reset instruction has been sent.");
            }

            var resetToken = AuthCacheKeys.NewSecureToken(32);
            var resetTokenHash = AuthCacheKeys.Sha256(resetToken);
            var resetKey = AuthCacheKeys.PasswordReset(emailHash, resetTokenHash);
            var ttl = TimeSpan.FromMinutes(Math.Max(1, _authSecurity.PasswordResetTtlMinutes));

            await _responseCache.SetCacheResponseAsync(resetKey, user.Id.ToString(), ttl);

            if (_emailService != null)
            {
                try
                {
                    var body = $"""
                        <p>Ban dang yeu cau dat lai mat khau Fabu.</p>
                        <p>Ma reset password cua ban:</p>
                        <p><strong>{System.Net.WebUtility.HtmlEncode(resetToken)}</strong></p>
                        <p>Ma het han sau {ttl.TotalMinutes:0} phut. Neu ban khong yeu cau, hay bo qua email nay.</p>
                        """;

                    await _emailService.SendEmailAsync(user.Email, "Fabu - Reset password", body);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not send password reset email for UserId {UserId}", user.Id);
                }
            }

            _logger.LogInformation("Password reset token issued. UserId: {UserId}, TTLMinutes: {TtlMinutes}",
                user.Id, ttl.TotalMinutes);

            return ApiResponse<bool>.Success(true, "If the email exists, a reset instruction has been sent.");
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (_responseCache == null)
                throw new InvalidOperationException("Response cache service is not initialized.");

            var normalizedEmail = NormalizeEmail(request.Email);
            var emailHash = AuthCacheKeys.Sha256(normalizedEmail);
            var tokenHash = AuthCacheKeys.Sha256(request.Token);
            var resetKey = AuthCacheKeys.PasswordReset(emailHash, tokenHash);
            var cachedUserId = await _responseCache.GetCachedResponseAsync<string>(resetKey);

            if (string.IsNullOrWhiteSpace(cachedUserId) || !long.TryParse(cachedUserId, out var userId))
                throw new AppException(ErrorCode.UNAUTHENTICATED, "Reset token invalid or expired.");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || !string.Equals(NormalizeEmail(user.Email), normalizedEmail, StringComparison.Ordinal))
                throw new AppException(ErrorCode.USER_NOT_EXISTED, "User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            await _responseCache.RemoveCacheResponseAsync(resetKey);
            await _responseCache.RemoveCacheResponseByGroupAsync(AuthCacheKeys.UserRefreshTokenGroup(user.Id));

            _logger.LogInformation("Password reset successfully. UserId: {UserId}", user.Id);

            return ApiResponse<bool>.Success(true, "Password reset successfully.");
        }

        private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

        private sealed class RefreshTokenCacheEntry
        {
            public long UserId { get; set; }
            public string TokenHash { get; set; } = string.Empty;
            public string SessionId { get; set; } = string.Empty;
            public string Provider { get; set; } = "password";
            public DateTimeOffset IssuedAt { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
        }
        //verify token - dung trong middleware - extract claims
    }
}
