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

        public Task<Payment> FindAsync(long id, bool includeAccount = false, bool includeService = false, bool includePostpaidBill = false, bool includeCustomer = false)
        {
            throw new NotImplementedException();
        }
    }
}
