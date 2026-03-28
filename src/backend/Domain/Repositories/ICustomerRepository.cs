using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface ICustomerRepository : IRepositoryBase<Customer, long>
    {
        Task<Customer?> GetByMobileNumberAsync(string mobileNumber);
        Task<IEnumerable<Customer>> GetByUserIdAsync(long userId);
        Task LinkUserToCustomerAsync(long customerId, long userId);
        Task<bool> ExistsByMobileAsync(string mobile);
        Task<List<Customer>> GetActiveCustomersAsync();
        Task<Customer?> GetWithAccountAsync(long customerId);
        Task<List<Customer>> GetTopCustomersBySpendingAsync(int top);
        Task<List<Customer>> GetCustomersWithUnpaidBillsAsync();
    }
}
