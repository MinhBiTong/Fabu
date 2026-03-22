using Domain.Entities;
using Domain.Repositories;
using Microsoft.Identity.Client;
using Persistence.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class AccountRepository : BaseRepository<Account, long>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context)
        {
        }

        public Task CreditAsync(long customerId, decimal amount)
        {
            throw new NotImplementedException();
        }

        public Task DebitAsync(long customerId, decimal amount)
        {
            throw new NotImplementedException();
        }

        public Task<Account?> GetByCustomerIdAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetCurrentBalanceAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasEnoughBalanceAsync(long customerId, decimal amount)
        {
            throw new NotImplementedException();
        }

        public Task LockAccountAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task UnlockAccountAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBalanceAsync(long accountId, decimal amount, bool isAdd = true)
        {
            throw new NotImplementedException();
        }
    }
}
