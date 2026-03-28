using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IPostpaidBillRepository : IRepositoryBase<PostpaidBill, long>
    {
        Task<List<PostpaidBill>> GetUnpaidBillsByCustomerAsync(long customerId);
        Task<PostpaidBill?> GetLatestBillAsync(long customerId);
        Task<decimal> GetTotalUnpaidAmountAsync(long customerId);
        Task<List<PostpaidBill>> GetOverdueBillsAsync();
        Task<bool> HasUnpaidBillAsync(long customerId);
        Task<List<PostpaidBill>> GetBillsByDateRangeAsync(long customerId, DateTime from, DateTime to);
    }
}
