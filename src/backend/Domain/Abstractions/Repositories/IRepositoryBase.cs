using Domain.Abstractions;
using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Repositories
{
    public interface IRepositoryBase<T, TKey> where T : class, IEntityBase<TKey> where TKey : struct
    {
        Task<T?> GetByIdAsync(TKey id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllPagedAsync(int pageNumber, int pageSize);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        void Delete(T entity);
        Task<bool> ExistsAsync(TKey id);
    }
}
