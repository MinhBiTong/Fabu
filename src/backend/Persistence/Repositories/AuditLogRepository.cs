using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;

namespace Persistence.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog, int>, IAuditLogRepository
    {
        public AuditLogRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<AuditLog>> GetByActionAsync(string action)
        {
            return _dbSet
                .Where(log => log.Action == action)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public Task<List<AuditLog>> GetByUserAsync(long userId)
        {
            return _dbSet
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public Task<List<AuditLog>> GetLogsByDateRangeAsync(DateTime from, DateTime to)
        {
            return _dbSet
                .Where(log => log.CreatedAt >= from && log.CreatedAt <= to)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityType, long entityId)
        {
            return await _dbSet
                .Where(log => log.EntityType == entityType && log.EntityId == entityId)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public Task<List<AuditLog>> GetRecentLogsAsync(int top)
        {
            return _dbSet
                .OrderByDescending(log => log.CreatedAt)
                .Take(top)
                .ToListAsync();
        }
    }
}
