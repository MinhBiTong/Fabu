
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> ExistsByMobileAsync(string mobile)
        {
            return await _dbSet.AnyAsync(c => c.MobileNumber == mobile && !c.IsDeleted);
        }

        public async Task<List<Customer>> GetActiveCustomersAsync()
        {
            return await _dbSet
                .Include(c => c.Account)
                .Where(c => !c.IsDeleted && c.Account != null && c.Account.Status == StatusAccount.Active)
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<Customer?> GetByMobileNumberAsync(string mobileNumber)
        {
            return await _dbSet
                .Include(c => c.Account)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.MobileNumber == mobileNumber && !c.IsDeleted);
        }

        public async Task<IEnumerable<Customer>> GetByUserIdAsync(long userId)
        {
            return await _dbSet
                .Include(c => c.Account)
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetCustomersWithUnpaidBillsAsync()
        {
            return await _dbSet
                .Include(c => c.PostpaidBills)
                .Where(c => !c.IsDeleted &&
                           c.PostpaidBills.Any(b => b.Status == StatusPostpaid.Unpaid ||
                                                   b.Status == StatusPostpaid.Overdue))
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetTopCustomersBySpendingAsync(int top)
        {
            return await _dbSet
                .Include(c => c.Transactions)
                .Where(c => !c.IsDeleted)
                .Select(c => new
                {
                    Customer = c,
                    TotalSpent = c.Transactions
                        .Where(t => t.Status == StatusTransaction.Success)
                        .Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(top)
                .Select(x => x.Customer)
                .ToListAsync();
        }

        public async Task<Customer?> GetWithAccountAsync(long customerId)
        {
            return await _dbSet
                .Include(c => c.Account)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == customerId && !c.IsDeleted);
        }

        public async Task LinkUserToCustomerAsync(long customerId, long userId)
        {
            var customer = await _dbSet.FindAsync(customerId);
            if (customer == null)
                throw new Exception("Customer not found");

            customer.UserId = userId;
            _dbSet.Update(customer);
        }
    }
}

