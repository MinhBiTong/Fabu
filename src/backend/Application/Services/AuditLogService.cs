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
        //inject
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IResponseCacheService? _responseCacheService;
        //private readonly IUserContext _userContext;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            //IUserContext userContext,
            ILogger<AuditLogService> logger)
            //IResponseCacheService? responseCacheService = null)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            //_responseCacheService = responseCacheService;
            //_userContext = userContext;
            _logger = logger;
        }

        public async Task<AuditLogResponse> CreateLogAsync(AuditLogCreateRequest request)
        {
            try
            {
                //validate
                //var existing = await _unitOfWork.AuditLogs.GetByUserAsync(request.UserId);
                //if (existing != null) throw new InvalidOperationException("Customer Log");
                var log = _mapper.Map<AuditLog>(request);

                //them vao Repository - luc nay chua luu xuong db
                await _unitOfWork.AuditLogs.AddAsync(log);

                //cuoi cung moi bam nut luu xuong db, neu bat ky dong nao o tren loi, DB se ko bi rac
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<AuditLogResponse>(log);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error while creating audit log");
                throw;
            }
        }

        public async Task DeleteLogAsync(int id)
        {
            var log = await _unitOfWork.AuditLogs.GetByIdAsync(id);
            if (log == null)
            {
                throw new KeyNotFoundException("Log not found");
            }
            _unitOfWork.AuditLogs.Delete(log);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<AuditLogResponse>> GetCurrentUserLogAsync(int UserId)
        {
            var log = await _unitOfWork.AuditLogs.GetByUserAsync(UserId);
            return _mapper.Map<List<AuditLogResponse>>(log);
        }

        public async Task<List<AuditLogResponse>> GetAllLogPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                int skip = (page - 1) * pageSize;
                var logs = await _unitOfWork.AuditLogs.GetAllPagedAsync(skip, pageSize);
                return _mapper.Map<List<AuditLogResponse>>(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllLogPagedAsync");
                return new List<AuditLogResponse>();
            }
        }
    }
}
