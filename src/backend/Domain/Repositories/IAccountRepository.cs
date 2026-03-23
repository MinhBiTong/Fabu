using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IAccountRepository : IRepositoryBase<Account, long>
    {
        Task<Account?> GetByCustomerIdAsync(long customerId);
        Task UpdateBalanceAsync(long accountId, decimal amount, bool isAdd = true);
        Task<decimal> GetCurrentBalanceAsync(long customerId);
        Task<bool> HasEnoughBalanceAsync(long customerId, decimal amount);
        Task CreditAsync(long customerId, decimal amount);
        Task DebitAsync(long customerId, decimal amount);
        Task LockAccountAsync(long customerId);
        Task UnlockAccountAsync(long customerId);
    }
}
