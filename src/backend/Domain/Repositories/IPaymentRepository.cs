using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IPaymentRepository : IRepositoryBase<Payment, long>
    {
        Task<List<Payment>> GetPaymentsByTransactionAsync(long transactionId);
        Task<Payment?> GetByTransactionRefAsync(string transactionRef);
        Task<bool> ExistsByTransactionRefAsync(string transactionRef);
        Task<List<Payment>> GetSuccessfulPaymentsAsync(long customerId);
        Task<decimal> GetTotalPaidAmountAsync(long customerId);
        Task<List<Payment>> GetFailedPaymentsAsync();
        Task<Payment?> GetLatestPaymentAsync(long customerId);
    }
}
