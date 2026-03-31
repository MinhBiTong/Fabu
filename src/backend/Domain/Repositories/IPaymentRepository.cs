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
        Task<Payment?> GetByPaymentRefAsync(string paymentRef);
        Task<bool> ExistsByPaymentRefAsync(string paymentRef);
        Task<List<Payment>> GetPaymentsByTransactionAsync(long transactionId);
        Task<List<Payment>> GetSuccessfulPaymentsAsync(long customerId);
        Task<List<Payment>> GetFailedPaymentsAsync();
        Task<Payment?> GetLatestPaymentAsync(long customerId);
        Task<decimal> GetTotalPaidAmountAsync(long customerId, DateTime? from = null, DateTime? to = null);
    }
}
