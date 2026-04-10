using Application.DTOs.Requests.RoleRequest;
using Application.DTOs.Responses.RoleResponse;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data.Contexts;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly IResponseCacheService _cache;
        private readonly ILogger<RoleService> _logger;
        private readonly AppDbContext _context;

        public RoleService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserContext userContext,
            IResponseCacheService cache,
            ILogger<RoleService> logger,
            AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext = userContext;
            _cache = cache;
            _logger = logger;
            _context = context;
        }
        public async Task<RoleResponse> CreateRoleAsync(RoleCreateRequest request)
        {
            try
            {
                //validate
                var existing = await _unitOfWork.Roles.GetByNameAsync(request.Name);
                if (existing != null) throw new InvalidOperationException("This role already exist");
                var log = _mapper.Map<Role>(request);

                //them vao Repository - luc nay chua luu xuong db
                await _unitOfWork.Roles.AddAsync(log);

                //cuoi cung moi bam nut luu xuong db, neu bat ky dong nao o tren loi, DB se ko bi rac
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<RoleResponse>(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new role");
                throw;
            }
        }

        public async Task DeleteRoleAsync(long id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null)
                throw new Exception("Role not exsited");

            _unitOfWork.Roles.Delete(role);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveCacheResponseByGroupAsync("roleGroup");
        }

        public async Task<List<RoleResponse>> GetAllRoleAsync()
        {
            string cacheKey = "roles:all";

            var cached = await _cache.GetCachedResponseAsync<List<RoleResponse>>(cacheKey);
            if (cached != null) return cached;

            var roles = await _unitOfWork.Roles.GetAllAsync();

            var result = _mapper.Map<List<RoleResponse>>(roles);

            await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<RoleResponse> GetRoleByNameAsync(string name)
        {
            string cacheKey = $"roles:{name}";

            var cached = await _cache.GetCachedResponseAsync<RoleResponse>(cacheKey);
            if (cached != null) return cached;

            var role = await _unitOfWork.Roles.GetByNameAsync(name);

            var result = _mapper.Map<RoleResponse>(role);

            await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<RoleResponse> UpdateRoleAsync(long id, RoleUpdateRequest request)
        {
            // 1. Lấy role + include permissions
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                throw new Exception("Role not found");

            var validPermissions = await _context.Permissions
                .Where(p => request.PermissionIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            if (validPermissions.Count != request.PermissionIds.Count)
            {
                throw new Exception("Permission unvail");
            }

            // 2. Update basic info
            role.Name = request.Name;
            role.Description = request.Description;

            // tránh null
            role.RolePermissions ??= new List<RolePermission>();

            // 3. Lấy danh sách PermissionId hiện tại
            var currentPermissionIds = role.RolePermissions
                .Where(rp => rp.PermissionId.HasValue)
                .Select(rp => rp.PermissionId!.Value)
                .ToHashSet();

            // 4. Permission mới từ request
            var newPermissionIds = request.PermissionIds;

            // 5. Tìm cái cần xóa
            var toRemove = currentPermissionIds.Except(newPermissionIds);

            // 6. Tìm cái cần thêm
            var toAdd = newPermissionIds.Except(currentPermissionIds);

            // 7. REMOVE
            role.RolePermissions = role.RolePermissions
                .Where(rp => !rp.PermissionId.HasValue || !toRemove.Contains(rp.PermissionId.Value))
                .ToList();

            // 8. ADD
            foreach (var permissionId in toAdd)
            {
                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }

            // 9. Save
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            return _mapper.Map<RoleResponse>(role);
        }
    }
}
