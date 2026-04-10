using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IServiceRepository : IRepositoryBase<Service, long>
    {
        Task<List<Service>> GetActiveServicesByCategoryAsync(string category);
        Task<List<Service>> GetPopularServicesAsync(int top);
        Task<Service?> GetByCodeAsync(string code);
        Task<bool> IsServiceActiveAsync(long serviceId);
        Task<List<Service>> SearchServicesAsync(string keyword);
        void Remove(Service service);
    }
}