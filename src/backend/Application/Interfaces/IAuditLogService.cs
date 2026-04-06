using Application.DTOs.Requests.AuditLogRequest;
using Application.DTOs.Responses.AuditLogResponse;

namespace Application.Interfaces
{
    public interface IAuditLogService
    {
        Task<AuditLogResponse> CreateLogAsync(AuditLogCreateRequest request);
        Task DeleteLogAsync(int id);
        Task<List<AuditLogResponse>> GetCurrentUserLogAsync();
        Task<List<AuditLogResponse>> GetAllLogPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
