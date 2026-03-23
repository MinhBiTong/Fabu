using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IRechargePlanRepository : IRepositoryBase<RechargePlan, long>
    {
        Task<List<RechargePlan>> GetActivePlansAsync();
        Task<RechargePlan?> GetByAmountAsync(decimal amount);
        Task<List<RechargePlan>> GetPlansByPriceRangeAsync(decimal min, decimal max);
        Task<List<RechargePlan>> GetPopularPlansAsync(int top);
        Task<List<RechargePlan>> GetPlansByProviderAsync(string provider);
        Task<bool> IsPlanActiveAsync(long planId);
    }
}
