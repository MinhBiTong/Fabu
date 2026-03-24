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
    public class PaymentRepository : BaseRepository<Payment, long>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public Task<bool> ExistsByTransactionRefAsync(string transactionRef)
        {
            throw new NotImplementedException();
        }

        public Task<Payment?> GetByTransactionRefAsync(string transactionRef)
        {
            throw new NotImplementedException();
        }

        public Task<List<Payment>> GetFailedPaymentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Payment?> GetLatestPaymentAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Payment>> GetPaymentsByTransactionAsync(long transactionId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Payment>> GetSuccessfulPaymentsAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalPaidAmountAsync(long customerId)
        {
            throw new NotImplementedException();
        }
    }
}
