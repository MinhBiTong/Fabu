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

        public Task<List<Domain.Entities.Transaction>> GetByUserIdAsync(long userId)
        {
            throw new NotImplementedException();
        }
    }
}
