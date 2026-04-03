using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;

namespace Persistence.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog, int>, IAuditLogRepository
    {
        public AuditLogRepository(AppDbContext context) : base(context) {}

        //lay log theo CRUD
        public async Task<List<AuditLog>> GetByActionAsync(string action)
        {
            return await _dbSet
                .Where(x => x.Action == action)
                .ToListAsync();
        }

        //lay tat ca log cua 1 user
        public async Task<List<AuditLog>> GetByUserAsync(long userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        //lay log tu ngay A → ngay B
        public async Task<List<AuditLog>> GetLogsByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _dbSet
                .Where(x => x.CreatedDate >= from && x.CreatedDate <= to)
                .ToListAsync();
        }

        //lay log cua entityType voi Id
        public async Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityType, long entityId)
        {
            return await _dbSet
                .Where(x => x.EntityType == entityType && x.EntityId == entityId)
                .ToListAsync();
        }

        //lay N log gan nhat
        public async Task<List<AuditLog>> GetRecentLogsAsync(int top)
        {
            return await _dbSet
                .OrderByDescending(x => x.CreatedDate)
                .Take(top)
                .ToListAsync();
        }
    }
}
