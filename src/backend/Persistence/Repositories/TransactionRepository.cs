using Domain.Entities;
using Domain.Repositories;
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

        public Task<bool> ExistsByTransactionRefAsync(string transactionRef)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Entities.Transaction?> GetByTransactionRefAsync(string transactionRef)
        {
            throw new NotImplementedException();
        }

        public Task<List<Domain.Entities.Transaction>> GetByUserIdAsync(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Domain.Entities.Transaction>> GetFailedTransactionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Entities.Transaction?> GetLatestSuccessfulTransactionAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Domain.Entities.Transaction>> GetPendingTransactionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Domain.Entities.Transaction>> GetRecentTransactionsAsync(long customerId, int top)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalAmountByCustomerAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Domain.Entities.Transaction>> GetTransactionsByCustomerAsync(long customerId, DateTime? from, DateTime? to)
        {
            throw new NotImplementedException();
        }
    }
}
