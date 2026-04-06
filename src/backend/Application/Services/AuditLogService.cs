using Application.DTOs.Requests.AuditLogRequest;
using Application.DTOs.Responses.AuditLogResponse;
using Application.Interfaces;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly IResponseCacheService _cache;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserContext userContext,
            IResponseCacheService cache,
            ILogger<AuditLogService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext = userContext;
            _cache = cache;
            _logger = logger;
        }

        public async Task<AuditLogResponse> CreateLogAsync(AuditLogCreateRequest request)
        {
            var log = _mapper.Map<AuditLog>(request);

            // nếu không truyền UserId → lấy từ token
            if (log.UserId == null && int.TryParse(_userContext.UserId, out var userId))
            {
                log.UserId = userId;
            }

            log.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.AuditLogs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            // clear cache liên quan
            await _cache.RemoveCacheResponseByGroupAsync("auditlogs");

            return _mapper.Map<AuditLogResponse>(log);
        }

        public async Task DeleteLogAsync(int id)
        {
            var log = await _unitOfWork.AuditLogs.GetByIdAsync(id);
            if (log == null)
                throw new Exception("Log not exsited");

            _unitOfWork.AuditLogs.Delete(log);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveCacheResponseByGroupAsync("auditlogs");
        }

        public async Task<List<AuditLogResponse>> GetCurrentUserLogAsync()
        {
            if (!int.TryParse(_userContext.UserId, out var userId))
                throw new Exception("User not found");

            string cacheKey = $"auditlogs:user:{userId}";

            var cached = await _cache.GetCachedResponseAsync<List<AuditLogResponse>>(cacheKey);
            if (cached != null) return cached;

            var logs = await _unitOfWork.AuditLogs
                .FindAsync(x => x.UserId == userId);

            var result = _mapper.Map<List<AuditLogResponse>>(logs);

            await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<List<AuditLogResponse>> GetAllLogPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                string cacheKey = $"auditlogs:page:{page}:{pageSize}";

                var cached = await _cache.GetCachedResponseAsync<List<AuditLogResponse>>(cacheKey);
                if (cached != null) return cached;

                int skip = (page - 1) * pageSize;

                var logs = await _unitOfWork.AuditLogs.GetAllPagedAsync(skip, pageSize);

                var result = _mapper.Map<List<AuditLogResponse>>(logs);

                await _cache.SetCacheResponseByGroupAsync(cacheKey, result, TimeSpan.FromMinutes(5));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllLogPagedAsync");
                return new List<AuditLogResponse>();
            }
        }
    }
}
