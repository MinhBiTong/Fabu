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
    public class PostpaidBillRepository : BaseRepository<PostpaidBill, long>, IPostpaidBillRepository
    {
        public PostpaidBillRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<PostpaidBill>> GetBillsByDateRangeAsync(long customerId, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<PostpaidBill?> GetLatestBillAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostpaidBill>> GetOverdueBillsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalUnpaidAmountAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostpaidBill>> GetUnpaidBillsByCustomerAsync(long customerId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasUnpaidBillAsync(long customerId)
        {
            throw new NotImplementedException();
        }
    }
}
