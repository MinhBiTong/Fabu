using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface ITransactionRepository : IRepositoryBase<Transaction, long>
    {
        Task<List<Transaction>> GetByUserIdAsync(long userId);
        Task<List<Transaction>> GetTransactionsByCustomerAsync(
           long customerId,
           DateTime? from,
           DateTime? to);

        Task<Transaction?> GetByTransactionRefAsync(string transactionRef);
        Task<bool> ExistsByTransactionRefAsync(string transactionRef);
        Task<List<Transaction>> GetRecentTransactionsAsync(long customerId, int top);
        Task<decimal> GetTotalAmountByCustomerAsync(long customerId, DateTime? from = null, DateTime? to = null);
        Task<List<Transaction>> GetFailedTransactionsAsync();
        Task<List<Transaction>> GetPendingTransactionsAsync();
        Task<Transaction?> GetLatestSuccessfulTransactionAsync(long customerId);
        Task<List<Transaction>> GetRechargeTransactionAsync(long customerId);
        Task<List<Transaction>> GetServiceActivationTransactionAsync(long customerId);
    }
}
