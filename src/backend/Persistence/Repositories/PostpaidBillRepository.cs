using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;

namespace Persistence.Repositories
{
    public class PostpaidBillRepository : BaseRepository<PostpaidBill, long>, IPostpaidBillRepository
    {
        public PostpaidBillRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<PostpaidBill>> GetBillsByDateRangeAsync(long customerId, DateTime from, DateTime to)
        {
            return _dbSet
                .Include(bill => bill.Payments)
                .Where(bill => bill.CustomerId == customerId && bill.BillMonth >= from && bill.BillMonth <= to)
                .OrderByDescending(bill => bill.BillMonth)
                .ToListAsync();
        }

        public Task<PostpaidBill?> GetLatestBillAsync(long customerId)
        {
            return _dbSet
                .Include(bill => bill.Payments)
                .Where(bill => bill.CustomerId == customerId)
                .OrderByDescending(bill => bill.BillMonth)
                .FirstOrDefaultAsync();
        }

        public Task<List<PostpaidBill>> GetOverdueBillsAsync()
        {
            var now = DateTime.UtcNow;
            return _dbSet
                .Include(bill => bill.Customer)
                .Where(bill => bill.Status != StatusPostpaid.Paid && bill.DueDate < now)
                .OrderBy(bill => bill.DueDate)
                .ToListAsync();
        }

        public Task<decimal> GetTotalUnpaidAmountAsync(long customerId)
        {
            return _dbSet
                .Where(bill => bill.CustomerId == customerId && bill.Status != StatusPostpaid.Paid)
                .SumAsync(bill => bill.TotalAmount - bill.PaidAmount);
        }

        public Task<List<PostpaidBill>> GetUnpaidBillsByCustomerAsync(long customerId)
        {
            return _dbSet
                .Include(bill => bill.Payments)
                .Where(bill => bill.CustomerId == customerId && bill.Status != StatusPostpaid.Paid)
                .OrderBy(bill => bill.DueDate)
                .ToListAsync();
        }

        public Task<bool> HasUnpaidBillAsync(long customerId)
        {
            return _dbSet.AnyAsync(bill => bill.CustomerId == customerId && bill.Status != StatusPostpaid.Paid);
        }
    }
}
