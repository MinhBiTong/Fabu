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
        Task<Payment> FindAsync(long id, bool includeAccount = false, bool includeService = false, bool includePostpaidBill = false, bool includeCustomer = false);

    }
}
