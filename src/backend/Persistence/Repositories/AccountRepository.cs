using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;

namespace Persistence.Repositories
{
    public class AccountRepository : BaseRepository<Account, long>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context)
        {
        }

        public async Task CreditAsync(long customerId, decimal amount)
        {
            var account = await GetByCustomerIdAsync(customerId)
                ?? throw new InvalidOperationException("Account not found.");

            EnsureActive(account);
            account.Balance += amount;
            account.LastRechargeDate = DateTime.UtcNow;
        }

        public async Task DebitAsync(long customerId, decimal amount)
        {
            var account = await GetByCustomerIdAsync(customerId)
                ?? throw new InvalidOperationException("Account not found.");

            EnsureActive(account);
            if (account.Balance < amount)
            {
                throw new InvalidOperationException("Account balance is not enough.");
            }

            account.Balance -= amount;
        }

        public Task<Account?> GetByCustomerIdAsync(long customerId)
        {
            return _dbSet
                .Include(account => account.Customer)
                .FirstOrDefaultAsync(account => account.CustomerId == customerId);
        }

        public async Task<decimal> GetCurrentBalanceAsync(long customerId)
        {
            var account = await GetByCustomerIdAsync(customerId);
            return account?.Balance ?? 0;
        }

        public async Task<bool> HasEnoughBalanceAsync(long customerId, decimal amount)
        {
            var account = await GetByCustomerIdAsync(customerId);
            return account is { Status: StatusAccount.Active } && account.Balance >= amount;
        }

        public async Task LockAccountAsync(long customerId)
        {
            var account = await GetByCustomerIdAsync(customerId)
                ?? throw new InvalidOperationException("Account not found.");

            account.Status = StatusAccount.Suspended;
        }

        public async Task UnlockAccountAsync(long customerId)
        {
            var account = await GetByCustomerIdAsync(customerId)
                ?? throw new InvalidOperationException("Account not found.");

            account.Status = StatusAccount.Active;
        }

        public async Task UpdateBalanceAsync(long accountId, decimal amount, bool isAdd = true)
        {
            var account = await GetByIdAsync(accountId)
                ?? throw new InvalidOperationException("Account not found.");

            EnsureActive(account);
            account.Balance = isAdd ? account.Balance + amount : account.Balance - amount;
            if (isAdd)
            {
                account.LastRechargeDate = DateTime.UtcNow;
            }
        }

        private static void EnsureActive(Account account)
        {
            if (account.Status != StatusAccount.Active)
            {
                throw new InvalidOperationException("Account is not active.");
            }
        }
    }
}
