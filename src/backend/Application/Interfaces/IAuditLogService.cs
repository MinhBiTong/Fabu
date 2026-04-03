using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Requests.AuditLogRequest;
using Application.DTOs.Responses.AuditLogResponse;

namespace Application.Interfaces
{
    public interface IAuditLogService
    {
        Task<AuditLogResponse> CreateLogAsync(AuditLogCreateRequest request);
        Task DeleteLogAsync(int id);
        Task<List<AuditLogResponse>> GetCurrentUserLogAsync(int UserId);
        Task<List<AuditLogResponse>> GetAllLogPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
