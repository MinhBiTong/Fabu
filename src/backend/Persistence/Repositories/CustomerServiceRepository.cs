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
    public class CustomerServiceRepository : BaseRepository<CustomerService, long>, ICustomerServiceRepository
    {
        public CustomerServiceRepository(AppDbContext context) : base(context)
        {
        }

        public Task CancelServiceAsync(long customerId, long serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CustomerService>> GetActiveServicesByCustomerAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CustomerService>> GetExpiringServicesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsServiceRegisteredAsync(long customerId, long serviceId)
        {
            throw new NotImplementedException();
        }

        public Task RegisterServiceAsync(long customerId, long serviceId)
        {
            throw new NotImplementedException();
        }
    }
}
