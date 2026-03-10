using Domain.Abstractions;
using Domain.Abstractions.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IUserContext _userContext; 

        public EfUnitOfWork(AppDbContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        //backing field de dung lazy loading cho repository
        private IUserRepository? _users;
        private IRoleRepository? _roles;
        private IPermissionRepository? _permissions;


        //trien khai property Users tu Interface
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
        public IPermissionRepository Permissions => _permissions ??= new PermissionRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            UpdateAuditFields();
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CommitAsync()
        {
            UpdateAuditFields();
            return await _context.SaveChangesAsync();
            //await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            //hoan tac cac thay doi trong Change Tracker neu co loi
            _context.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Unchanged);
            await Task.CompletedTask;
        }

        public void Dispose() => _context.Dispose();

        public Task<int> CommitAsync(int commitId)
        {
            throw new NotImplementedException();
        }

        private void UpdateAuditFields()
        {
            var userId = _userContext.UserId ?? "System";
            var now = DateTimeOffset.UtcNow;

            foreach (var entry in _context.ChangeTracker.Entries())
            {
                //xu ly soft delete
                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete softDeleteEntity)
                {
                    //chuyen tu xoa sang cap nhat
                    entry.State = EntityState.Modified;
                    softDeleteEntity.IsDeleted = true;
                    softDeleteEntity.DeletedAt = now;
                }

                //xu ly date tracking
                if (entry.Entity is IDateTracking dateEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        dateEntity.CreatedDate = now;
                    }
                    dateEntity.ModifiedDate = now;
                }

                //xu ly user tracking
                if (entry.Entity is IUserTracking userEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        userEntity.CreatedBy = userId;
                    }
                    userEntity.ModifiedBy = userId;
                }
            }
        }
    }
}
