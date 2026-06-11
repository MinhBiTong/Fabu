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
    public class PaymentRepository : BaseRepository<Payment, long>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByPaymentRefAsync(string paymentRef)
        {
            return await _dbSet.AnyAsync(p => p.PaymentRef == paymentRef && !p.IsDeleted);
        }

        public async Task<Payment?> GetByPaymentRefAsync(string paymentRef)
        {
            return await _dbSet
            .Include(p => p.Transactions)
                .ThenInclude(t => t.Service)
            .Include(p => p.Transactions)
                .ThenInclude(t => t.Order)
                    .ThenInclude(o => o!.Items)
                        .ThenInclude(i => i.Product)
            .Include(p => p.Orders)
                .ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product)
            .Include(p => p.PostpaidBill)
            .FirstOrDefaultAsync(p => p.PaymentRef == paymentRef && !p.IsDeleted);
        }

        public async Task<List<Payment>> GetFailedPaymentsAsync()
        {
            return await _dbSet
                .Where(p => p.Status == StatusPayment.Failed && !p.IsDeleted)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment?> GetLatestPaymentAsync(long customerId)
        {
            return await _dbSet
                .Where(p => p.Transactions.Any(t => t.CustomerId == customerId) && !p.IsDeleted)
                .OrderByDescending(p => p.PaymentDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Payment>> GetPaymentsByTransactionAsync(long transactionId)
        {
            return await _dbSet
                .Include(p => p.Transactions)
                .Where(p => p.Transactions.Any(t => t.Id == transactionId) && !p.IsDeleted)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetSuccessfulPaymentsAsync(long customerId)
        {
            return await _dbSet
                .Where(p => p.Transactions.Any(t => t.CustomerId == customerId)
                    && p.Status == Domain.ValueObjects.StatusPayment.Completed
                    && !p.IsDeleted)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaidAmountAsync(long customerId, DateTime? from = null, DateTime? to = null)
        {
            var query = _dbSet
                .Where(p => p.Transactions.Any(t => t.CustomerId == customerId)
                    && p.Status == StatusPayment.Completed    
                    && !p.IsDeleted);

            if (from.HasValue) query = query.Where(p => p.PaymentDate >= from.Value);
            if (to.HasValue) query = query.Where(p => p.PaymentDate <= to.Value);

            return await query.SumAsync(p => p.Amount);
        }
    }
}
