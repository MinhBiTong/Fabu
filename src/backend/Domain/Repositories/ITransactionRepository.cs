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
        Task<Transaction> GetByIdAsync(long id);
        Task<List<Transaction>> GetByUserIdAsync(long userId);

    }
}
