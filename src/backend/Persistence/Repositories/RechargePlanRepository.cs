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
    public class RechargePlanRepository : BaseRepository<RechargePlan, long>, IRechargePlanRepository
    {
        public RechargePlanRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<RechargePlan>> GetActivePlansAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RechargePlan?> GetByAmountAsync(decimal amount)
        {
            throw new NotImplementedException();
        }

        public Task<List<RechargePlan>> GetPlansByPriceRangeAsync(decimal min, decimal max)
        {
            throw new NotImplementedException();
        }

        public Task<List<RechargePlan>> GetPlansByProviderAsync(string provider)
        {
            throw new NotImplementedException();
        }

        public Task<List<RechargePlan>> GetPopularPlansAsync(int top)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsPlanActiveAsync(long planId)
        {
            throw new NotImplementedException();
        }
    }
}
