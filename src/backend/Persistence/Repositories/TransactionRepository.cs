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
using System.Transactions;

namespace Persistence.Repositories
{
    public class TransactionRepository : BaseRepository<Domain.Entities.Transaction, long>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByTransactionRefAsync(string transactionRef)
        {
            return await _dbSet.AnyAsync(t => t.TransactionRef == transactionRef && !t.IsDeleted);
        }

        public async Task<Domain.Entities.Transaction?> GetByTransactionRefAsync(string transactionRef)
        {
            return await _dbSet
                .Include(t => t.Customer)
                .Include(t => t.CouponUsages)
                .FirstOrDefaultAsync(t => t.TransactionRef == transactionRef && !t.IsDeleted);
        }

        public async Task<List<Domain.Entities.Transaction>> GetByUserIdAsync(long userId)
        {
            return await _dbSet
                .Include(t => t.Customer)
                .Where(t => t.Customer != null && t.Customer.UserId == userId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Domain.Entities.Transaction>> GetFailedTransactionsAsync()
        {
            return await _dbSet
                .Where(t => t.Status == StatusTransaction.Failed && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Transaction?> GetLatestSuccessfulTransactionAsync(long customerId)
        {
            return await _dbSet
                .Where(t => t.CustomerId == customerId
                    && t.Status == StatusTransaction.Success
                    && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Domain.Entities.Transaction>> GetPendingTransactionsAsync()
        {
            return await _dbSet
                .Where(t => t.Status == Domain.ValueObjects.StatusTransaction.Pending && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Domain.Entities.Transaction>> GetRecentTransactionsAsync(long customerId, int top)
        {
            return await _dbSet
                .Include(t => t.CouponUsages)
                .Where(t => t.CustomerId == customerId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedDate)
                .Take(top)
                .ToListAsync();
        }

        public async Task<List<Domain.Entities.Transaction>> GetRechargeTransactionAsync(long customerId)
        {
            return await _dbSet
                .Where(t => t.CustomerId == customerId
                    && t.TransactionType == "Recharge"
                    && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Domain.Entities.Transaction>> GetServiceActivationTransactionAsync(long customerId)
        {
            return await _dbSet
                .Where(t => t.CustomerId == customerId
                         && t.TransactionType == "ServiceActivation"
                         && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalAmountByCustomerAsync(long customerId, DateTime? from = null, DateTime? to = null)
        {
            var query = _dbSet
                .Where(t => t.CustomerId == customerId
                    && t.Status == StatusTransaction.Success
                    && !t.IsDeleted);

            if(from.HasValue) query = query.Where(t => t.CreatedDate >= from.Value);
            if (to.HasValue) query = query.Where(t => t.CreatedDate <= to.Value);

            return await query.SumAsync(t => t.Amount);
        }

        public async Task<List<Domain.Entities.Transaction>> GetTransactionsByCustomerAsync(long customerId, DateTime? from, DateTime? to)
        {
            var query = _dbSet
                .Include(t => t.CouponUsages)
                .Where(t => t.CustomerId == customerId && !t.IsDeleted);

            if (from.HasValue)
                query = query.Where(t => t.CreatedDate >= from.Value);
            if (to.HasValue)
                query = query.Where(t => t.CreatedDate <= to.Value);

            return await query
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }
}
