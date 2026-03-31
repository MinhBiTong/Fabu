using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IAuditLogRepository : IRepositoryBase<AuditLog, int>
    {
        Task<List<AuditLog>> GetByUserAsync(long userId);
        Task<List<AuditLog>> GetByActionAsync(string action);
        Task<List<AuditLog>> GetRecentLogsAsync(int top);
        Task<List<AuditLog>> GetLogsByDateRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityType, long entityId);
    }
}
