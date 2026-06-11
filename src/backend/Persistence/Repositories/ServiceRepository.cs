
using Domain.Abstractions;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Service>> GetActiveServicesByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(service => service.IsActive && service.Category == category)
                .OrderBy(service => service.Price)
                .ToListAsync();
        }

        public async Task<Service?> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(service => service.ServiceCode == code);
        }

        public async Task<List<Service>> GetPopularServicesAsync(int top)
        {
            return await _dbSet
                .Where(service => service.IsActive)
                .OrderByDescending(service => service.CustomerServices.Count)
                .ThenBy(service => service.Price)
                .Take(Math.Clamp(top, 1, 50))
                .ToListAsync();
        }

        public async Task<bool> IsServiceActiveAsync(long serviceId)
        {
            return await _dbSet.AnyAsync(service => service.Id == serviceId && service.IsActive);
        }

        public void Remove(Service service)
        {
            Delete(service);
        }

        public async Task<List<Service>> SearchServicesAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await _dbSet
                    .Where(service => service.IsActive)
                    .OrderBy(service => service.ServiceName)
                    .ToListAsync();
            }

            var normalized = keyword.Trim();
            return await _dbSet
                .Where(service => service.IsActive
                    && (service.ServiceName.Contains(normalized)
                        || service.ServiceCode.Contains(normalized)
                        || service.Category.Contains(normalized)
                        || service.Description.Contains(normalized)))
                .OrderBy(service => service.Price)
                .ToListAsync();
        }
    }
}

