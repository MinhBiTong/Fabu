using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstractions.Repositories;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICustomerServiceRepository : IRepositoryBase<CustomerService, long>
    {
        Task<List<CustomerService>> GetActiveServicesByCustomerAsync(long customerId);
        Task<bool> IsServiceRegisteredAsync(long customerId, long serviceId);
        Task RegisterServiceAsync(long customerId, long serviceId);
        Task CancelServiceAsync(long customerId, long serviceId);
        Task<List<CustomerService>> GetExpiringServicesAsync();
    }
}
