using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IUserRepository : IRepositoryBase<User, long>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetActiveUserAsync();
    }
}
