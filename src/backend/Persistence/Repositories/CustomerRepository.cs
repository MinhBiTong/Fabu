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
    public class CustomerRepository : BaseRepository<Customer, long>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }

        public Task<bool> ExistsByMobileAsync(string mobile)
        {
            throw new NotImplementedException();
        }

        public Task<List<Customer>> GetActiveCustomersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Customer?> GetByMobileNumberAsync(string mobileNumber)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Customer>> GetByUserIdAsync(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Customer>> GetCustomersWithUnpaidBillsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Customer>> GetTopCustomersBySpendingAsync(int top)
        {
            throw new NotImplementedException();
        }

        public Task<Customer?> GetWithAccountAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task LinkUserToCustomerAsync(long customerId, long userId)
        {
            throw new NotImplementedException();
        }
    }
}
