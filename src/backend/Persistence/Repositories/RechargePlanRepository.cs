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
    }
}
