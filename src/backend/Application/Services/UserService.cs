using Application.DTOs.Requests.UserRequest;
using Application.DTOs.Responses.UserResponse;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Domain.Abstractions;
using Serilog.Core;
using Persistence.Data.Configurations;
namespace Application.Services
{
    public class UserService : IUserService
    {
        //inject
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IResponseCacheService _responseCacheService;
        private readonly IUserContext _userContext;
        private readonly Logger _logger;
        private readonly UserConfiguration _userConfiguration;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IResponseCacheService? responseCacheService, IUserContext userContext, Logger logger, UserConfiguration _userConfiguration)
        {
            
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _responseCacheService = responseCacheService;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            //validate
            var existing = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (existing != null) throw new InvalidOperationException("Email exists");

            var user = new User { Username = request.Username, Email = request.Email, PasswordHash = request.PasswordHash };
            //them vao Repository - luc nay chua luu xuong db
            await _unitOfWork.Users.AddAsync(user);
            //await _unitOfWork.Orders.AddAsync(order);
            //await _unitOfWork.Roles.AddAsync(role);

            //cuoi cung moi bam nut luu xuong db, neu bat ky dong nao o tren loi, DB se ko bi rac
            await _unitOfWork.CommitAsync();
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserResponse>(user);
        }

        public Task<UserResponse> DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserResponse?> GetCurrentUserAsync() //tu claim
        {
            var userClaim = _userContext.UserId;
            if (string.IsNullOrEmpty(userClaim) || !int.TryParse(userClaim, out var userId)) 
                throw new InvalidOperationException("Invalid token claims");

            var cacheKey = $"user:{userId}";
            var cacheUser = await _responseCacheService.GetCachedResponseAsync<UserResponse>(cacheKey);
            if (cacheUser != null) return cacheUser;

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return null;

            var response = _mapper.Map<UserResponse>(user);
            await _responseCacheService.SetCacheResponseByGroupAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            return response;
        }

        public Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<UserResponse> GetByIdAsync(int id)
        {
            var cacheKey = $"user:{id}";
            var cached = await _responseCacheService.GetCachedResponseAsync<UserResponse>(cacheKey);
            if (cached != null) return cached;

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user != null) return null;

            var response = _mapper.Map<UserResponse>(user);
            await _responseCacheService.SetCacheResponseByGroupAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            return response;
        }

        public async Task<List<UserResponse>> GetAllUsersPagedAsync(
            int page,
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                int skip = (page - 1) * pageSize;
                var users = await _unitOfWork.Users.GetAllPagedAsync(skip, pageSize);
                return _mapper.Map<List<UserResponse>>(users);
            } catch (Exception ex)
            {
                _logger.Error("Error in GetAllUsersPagedAsync: {Message}", ex.Message);
            }
            return new List<UserResponse>();
        }
    }
}
