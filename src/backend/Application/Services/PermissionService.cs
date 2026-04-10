using Application.DTOs.Requests.PermissionRequest;
using Application.DTOs.Responses.PermissionResponse;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data.Contexts;

namespace Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly IResponseCacheService _cache;
        private readonly ILogger<AuditLogService> _logger;
        private readonly AppDbContext _context;


        public PermissionService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserContext userContext,
            IResponseCacheService cache,
            ILogger<AuditLogService> logger,
            AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext = userContext;
            _cache = cache;
            _logger = logger;
            _context = context;

        }
        public async Task<PermissionResponse> CreatePermissionAsync(PermissionCreateRequest request)
        {
            try
            {
                //validate
                var existing = await _unitOfWork.Permissions.GetByPermissionNameAsync(request.Name);
                if (existing != null) throw new InvalidOperationException("This permission already exist");
                var permission = _mapper.Map<Permission>(request);

                //them vao Repository - luc nay chua luu xuong db
                await _unitOfWork.Permissions.AddAsync(permission);

                //cuoi cung moi bam nut luu xuong db, neu bat ky dong nao o tren loi, DB se ko bi rac
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<PermissionResponse>(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new permission");
                throw;
            }
        }

        public async Task DeletePermissionAsync(int id)
        {
            var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
            if (permission == null)
                throw new Exception("Permission not exsited");

            _unitOfWork.Permissions.Delete(permission);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveCacheResponseByGroupAsync("PermissionGroup");
        }

        public async Task<List<PermissionResponse>> GetAllPermissionAsync()
        {
            string cacheKey = "permissions:all";

            var cached = await _cache.GetCachedResponseAsync<List<PermissionResponse>>(cacheKey);
            if (cached != null) return cached;

            var roles = await _unitOfWork.Permissions.GetAllAsync();

            var result = _mapper.Map<List<PermissionResponse>>(roles);

            await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<PermissionResponse> GetPermissionByNameAsync(string name)
        {
            string cacheKey = $"permission:{name}";

            var cached = await _cache.GetCachedResponseAsync<PermissionResponse>(cacheKey);
            if (cached != null) return cached;

            var role = await _unitOfWork.Permissions.GetByPermissionNameAsync(name);

            var result = _mapper.Map<PermissionResponse>(role);

            await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<PermissionResponse> UpdatePermissionAsync(int id, PermissionUpdateRequest request)
        {
            try
            {
                // 1. Lấy permission từ DB
                var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
                if (permission == null)
                    throw new Exception("Permission not found");

                // 2. Update field đơn giản
                permission.Name = request.Name;
                permission.Description = request.Description;

                // 3. Update
                _unitOfWork.Permissions.Update(permission);

                // 4. Save DB
                await _unitOfWork.SaveChangesAsync();

                // 5. Map sang response
                return _mapper.Map<PermissionResponse>(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new permission");
                throw;
            }
        }
    }
}
