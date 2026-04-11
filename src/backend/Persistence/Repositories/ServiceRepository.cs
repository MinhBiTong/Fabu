
using Domain.Abstractions;
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
    public class ServiceRepository : BaseRepository<Service, long>, IServiceRepository

    {
        public ServiceRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<Service>> GetActiveServicesByCategoryAsync(string category)
        {
            throw new NotImplementedException();
        }

        public Task<Service?> GetByCodeAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task<List<Service>> GetPopularServicesAsync(int top)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsServiceActiveAsync(long serviceId)
        {
            throw new NotImplementedException();
        }

        public void Remove(Service service)
        {
            throw new NotImplementedException();
        }

        public Task<List<Service>> SearchServicesAsync(string keyword)
        {
            throw new NotImplementedException();
        }
    }
}

