using Application.DTOs.Requests.LoginRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.LoginResponse;
using Application.Interfaces;
using Azure.Core;
using Domain.Abstractions;
using Domain.Configurations;
using Domain.Entities;
using Domain.Exceptions;
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
        private readonly TimeSpan _refreshTokenExpiry = TimeSpan.FromDays(7); //7 days
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<LoginRequest> _validator;
        private readonly ILogger<AuthService> _logger;
        private readonly ICustomerService _customerService;

        public AuthService(
            IConfiguration configuration,
            IValidator<LoginRequest> validator,
            IOptions<JwtConfiguration> jwtOptions,
            IUnitOfWork unitOfWork,
            ILogger<AuthService> logger,
            ICustomerService customerService,
            IResponseCacheService? responseCache = null)
        {
            _configuration = configuration;
            _responseCache = responseCache;
            _jwtConfiguration = jwtOptions.Value;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
            _customerService = customerService;
        }

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
            // Giả sử OTP đã được validate ở middleware hoặc service riêng (OTPService)
            // Ở đây chúng ta chỉ xử lý business sau khi OTP đúng

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                throw new AppException(ErrorCode.USER_NOT_EXISTED);

            if (user.IsActive)
                return new VerifyOtpResponse { Success = true, Message = "The account was previously activated." };

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
                var key = $"otp:verify:{user.Id}";
                await _responseCache.RemoveCacheResponseAsync(key);
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
            // TODO: Tích hợp SMS gateway thực tế (SpeedSMS, Twilio, ...)
            // Hiện tại mock OTP
            var otp = new Random().Next(100000, 999999).ToString();
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            var key = $"otp:verify:{userId}";
            await _responseCache.SetCacheResponseAsync(key, otp, TimeSpan.FromMinutes(5));

            _logger.LogInformation("OTP sent to phone {Phone} for user {UserId}. OTP: {Otp} (mock)", phoneNumber, userId, otp);
        }

        public async Task<ApiResponse<bool>> ResendOtpAsync(ResendOtpRequest request)
        {
            // 1. Tìm User theo số điện thoại hoặc UserId
            var user = await _unitOfWork.Users.GetByMobileNumberAsync(request.PhoneNumber);
            if (user == null) return ApiResponse<bool>.Fail(404, "Phone number hasn't been register");

            // 2. Nếu tài khoản đã active rồi thì không gửi lại làm gì
            if (user.IsActive) return ApiResponse<bool>.Fail(400, "The account was previously activated.");

            // 3. (Optional) Check Rate Limit - Tránh spam SMS tốn tiền
            var rateLimitKey = $"otp:limit:{user.Id}";
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

            //generate claims + scope
            var claims = await GenerateClaimsAsync(user);
            var scope = await BuildScope(user); //custom scope dua vao role
            claims.Add(new Claim("scope", scope)); //add scope as claim

            //generate tokens
            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken(); //random string

            //store refresh in redis
            await StoreRefreshTokenAsync(user.Id, refreshToken, TimeSpan.FromDays(7));

            //response
            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken, // tra cho client luu secure storage
                ExpiresAt = DateTime.Now.AddMinutes(_jwtConfiguration.ExpiryMinutes), //access expiry
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

            //add roles
            var roles = user.UserRoles?.Select(ur => ur.Role?.Name).Where(role => role != null).ToList() ?? new List<string?>();
            //var roles = rolesTask.Result;
            //claims.AddRange(rolesTask.Select(role => new Claim(ClaimTypes.Role, role)));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
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
            var roles = user.UserRoles?.Select(ur => ur.Role?.Name).Where(role => role != null).ToList() ?? new List<string>();// Removed `.Result` to fix the issue
            var scopes = new List<string>();

            if (roles.Contains("Admin"))
            {
                scopes.AddRange(new[] { "read:all", "write:all", "delete:all" });
            }
            else if (roles.Contains("Customer"))
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
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        //store rt trong redis - key: "refresh:{userId}:{hash}", value: expiry -> expiry
        private async Task StoreRefreshTokenAsync(long userId, string refreshToken, TimeSpan expiry)
        {
            if (_responseCache == null) return;

            var refreshHash = HashToken(refreshToken);

            // Key chính - dễ tìm
            var mainKey = $"refresh:{userId}:{refreshHash}";

            // Value = userId (string)
            var value = userId.ToString();

            // Lưu với thời hạn tự động xóa
            await _responseCache.SetCacheResponseByGroupAsync(
                mainKey,
                value,
                absoluteExpiry: expiry
            );

            // Lưu thêm key đơn giản chỉ chứa hash (dễ tìm khi chỉ có refreshToken)
            var simpleKey = $"refreshToken:{refreshHash}";
            await _responseCache.SetCacheResponseByGroupAsync(
                simpleKey,
                value,
                absoluteExpiry: expiry
            );

            // Thêm vào group để logout dễ xóa
            var groupKey = $"group:refresh:{userId}";
            await _responseCache.AddToGroupAsync(groupKey, mainKey);

            _logger.LogInformation("Stored refresh token - MainKey: {MainKey} | SimpleKey: {SimpleKey} | UserId: {UserId}",
                mainKey, simpleKey, userId);
            //--------
            //var groupKey = $"Group:refresh:{userId}";
            //_logger.LogInformation("SAVING to Redis - Key: {Key}, Value: {Value}", key, expiryTicks);
            //await _responseCache.AddToGroupAsync(groupKey, key);

            //if (_responseCache == null) return;
            //var key = $"refresh:{userId}:{refreshHash}";
            //var expiryTicks = DateTime.UtcNow.Add(expiry).Ticks.ToString();

            ////luu token
            //await _responseCache.SetRawStringAsync(key, expiryTicks, expiry);

            //// Thêm key vào group của user qua service response cache
            //var groupKey = $"Group:refresh:{userId}";
            //await _responseCache.AddToGroupAsync(groupKey, key);
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                throw new UnauthorizedAccessException("Refresh token is required");

            if (_responseCache == null)
            {
                throw new InvalidOperationException("Response cache service is not initialized.");
            }

            var refreshHash = HashToken(request.RefreshToken);
            var key = $"refresh:{request.UserId}:{refreshHash}";

            _logger.LogInformation("Attempting to refresh token with Key: {Key}", key);

            var cachedUserId = await _responseCache.GetCachedResponseAsync<string>(key);
            if (string.IsNullOrEmpty(cachedUserId) || cachedUserId != request.UserId.ToString())
            {
                _logger.LogWarning("Refresh Token not found or mismatched in Redis. Key: {Key}", key);
                throw new AppException(ErrorCode.UNAUTHENTICATED, "Invalid or expired refresh token");
            }

            await _responseCache.RemoveCacheResponseAsync(key);
            await _responseCache.RemoveCacheResponseAsync($"refreshToken:{refreshHash}");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                throw new AppException(ErrorCode.USER_NOT_EXISTED, "User not found");

            var claims = await GenerateClaimsAsync(user);
            var scope = await BuildScope(user);
            claims.Add(new Claim("scope", scope));

            var newAccessToken = GenerateAccessToken(claims);
            var newRefreshToken = GenerateRefreshToken();

            await StoreRefreshTokenAsync(user.Id, newRefreshToken, TimeSpan.FromDays(7));

            _logger.LogInformation("Refresh successful for User {UserId}. New RT created.", user.Id);

            return new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(_jwtConfiguration.ExpiryMinutes),
                Claims = claims.Select(c => new LoginResponse.ClaimDto
                {
                    Type = c.Type,
                    Value = c.Value
                }).ToList()
            };
        }

        public async Task<ApiResponse<LoginResponse>> RefreshTokenFromCookieAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is required");

            if (_responseCache == null)
                throw new InvalidOperationException("Response cache service is not initialized.");

            var refreshHash = HashToken(refreshToken);

            // Tìm userId từ Redis
            var userIdStr = await _responseCache.GetCachedResponseAsync<string>($"refreshToken:{refreshHash}");

            if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out long userId))
                throw new UnauthorizedAccessException("Refresh token not exists or has expired");

            await _responseCache.RemoveCacheResponseAsync($"refresh:{userId}:{refreshHash}");
            await _responseCache.RemoveCacheResponseAsync($"refreshToken:{refreshHash}");

            // Lấy user từ DB
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(ErrorCode.USER_NOT_EXISTED);

            // Tạo token mới
            var claims = await GenerateClaimsAsync(user);
            var scope = await BuildScope(user);
            claims.Add(new Claim("scope", scope));
            var newAccessToken = GenerateAccessToken(claims);
            var newRefreshToken = GenerateRefreshToken();

            // Lưu refresh token mới
            await StoreRefreshTokenAsync(user.Id, newRefreshToken, TimeSpan.FromDays(7));

            return ApiResponse<LoginResponse>.Success(new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtConfiguration.ExpiryMinutes),
                Claims = claims.Select(c => new LoginResponse.ClaimDto
                {
                    Type = c.Type,
                    Value = c.Value
                }).ToList()
            });
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

            var claims = await GenerateClaimsAsync(user);
            claims.Add(new Claim("idp", provider));
            claims.Add(new Claim("amr", "external"));

            var scope = await BuildScope(user);
            claims.Add(new Claim("scope", scope));

            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken();
            await StoreRefreshTokenAsync(user.Id, refreshToken, _refreshTokenExpiry);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtConfiguration.ExpiryMinutes),
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
                // === XỬ LÝ REFRESH TOKEN TỪ COOKIE ===
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var refreshHash = HashToken(refreshToken);

                    // Tìm userId từ Redis (dựa trên cách em lưu)
                    var userIdStr = await _responseCache.GetCachedResponseAsync<string>($"refreshToken:{refreshHash}");

                    if (!string.IsNullOrEmpty(userIdStr) && long.TryParse(userIdStr, out long userId))
                    {
                        // Xóa toàn bộ refresh token của user này
                        var groupKey = $"group:refresh:{userId}";
                        await _responseCache.RemoveCacheResponseByGroupAsync(groupKey);

                        _logger.LogInformation("Cleared all refresh tokens for user {UserId}", userId);
                    }
                }

                // === BLACKLIST ACCESS TOKEN (nếu có) ===
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(accessToken))
                    {
                        var jwt = handler.ReadJwtToken(accessToken);
                        var remaining = jwt.ValidTo - DateTime.UtcNow;

                        if (remaining > TimeSpan.Zero)
                        {
                            await _responseCache.SetCacheResponseByGroupAsync(
                                $"blacklist:at:{accessToken}",
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
        //verify token - dung trong middleware - extract claims
    }
}
