using Domain.Abstractions.Entities;
using Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public abstract class BaseRepository<T, TKey> : IRepositoryBase<T, TKey>
            where T : class, IEntityBase<TKey>
            where TKey : struct
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context) => _context = context;

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task<T?> GetByIdAsync(TKey id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<List<T>> GetAllPagedAsync(int skip, int take)
        {
            return await _context.Set<T>().Skip(skip).Take(take).ToListAsync();
        }

        public async Task<bool> ExistsAsync(TKey id)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id.Equals(id));
        }
    }
}
