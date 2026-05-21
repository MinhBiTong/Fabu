using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;

namespace Persistence.Repositories
{
    public class RechargePlanRepository : BaseRepository<RechargePlan, long>, IRechargePlanRepository
    {
        public RechargePlanRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<RechargePlan>> GetActivePlansAsync()
        {
            return _dbSet
                .Where(plan => plan.IsActive)
                .OrderBy(plan => plan.Amount)
                .ToListAsync();
        }

        public Task<RechargePlan?> GetByAmountAsync(decimal amount)
        {
            return _dbSet
                .Where(plan => plan.IsActive && plan.Amount == amount)
                .OrderByDescending(plan => plan.BonusAmount)
                .FirstOrDefaultAsync();
        }

        public Task<List<RechargePlan>> GetPlansByPriceRangeAsync(decimal min, decimal max)
        {
            return _dbSet
                .Where(plan => plan.IsActive && plan.Amount >= min && plan.Amount <= max)
                .OrderBy(plan => plan.Amount)
                .ToListAsync();
        }

        public Task<List<RechargePlan>> GetPlansByProviderAsync(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return GetActivePlansAsync();
            }

            return _dbSet
                .Where(plan => plan.IsActive &&
                    (plan.PlanName.Contains(provider) || plan.Description.Contains(provider)))
                .OrderBy(plan => plan.Amount)
                .ToListAsync();
        }

        public Task<List<RechargePlan>> GetPopularPlansAsync(int top)
        {
            return _dbSet
                .Where(plan => plan.IsActive)
                .OrderByDescending(plan => plan.BonusAmount)
                .ThenBy(plan => plan.Amount)
                .Take(top)
                .ToListAsync();
        }

        public Task<bool> IsPlanActiveAsync(long planId)
        {
            return _dbSet.AnyAsync(plan => plan.Id == planId && plan.IsActive);
        }
    }
}
