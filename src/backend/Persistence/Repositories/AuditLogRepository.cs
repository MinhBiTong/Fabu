using Domain.Entities;
using Domain.Repositories;
using Persistence.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog, int>, IAuditLogRepository
    {
        public AuditLogRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<AuditLog>> GetByActionAsync(string action)
        {
            throw new NotImplementedException();
        }

        public Task<List<AuditLog>> GetByUserAsync(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AuditLog>> GetLogsByDateRangeAsync(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityType, long entityId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AuditLog>> GetRecentLogsAsync(int top)
        {
            throw new NotImplementedException();
        }
    }
}
