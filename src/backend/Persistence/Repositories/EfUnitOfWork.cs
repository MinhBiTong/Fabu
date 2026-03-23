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
        private IAccountRepository? _accounts;
        private IAuditLogRepository? _auditLogs;
        private ICouponRepository? _coupons;
        private ICouponUsageRepository? _couponUsages;
        private ICustomerRepository? _customers;
        private ICustomerServiceRepository? _customerServices;
        private IPaymentRepository? _payments;
        private IPostpaidBillRepository? _postpaidBills;
        private IServiceRepository? _services;
        private IFeedbackRepository? _feedbacks;
        private IRechargePlanRepository? _rechargePlans;
        private ITransactionRepository? _transactions;


        //trien khai property Users tu Interface
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
        public IPermissionRepository Permissions => _permissions ??= new PermissionRepository(_context);
        public IAccountRepository Accounts => _accounts ??= new AccountRepository(_context);
        public IAuditLogRepository AuditLogs => _auditLogs ??= new AuditLogRepository(_context);
        public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
        public ICustomerServiceRepository CustomerServices => _customerServices ??= new CustomerServiceRepository(_context);
        public ICouponRepository Coupons => _coupons ??= new CouponRepository(_context);
        public ICouponUsageRepository CouponUsages => _couponUsages ??= new CouponUsageRepository(_context);
        public IPostpaidBillRepository PostpaidBills => _postpaidBills ??= new PostpaidBillRepository(_context);
        public IServiceRepository Services => _services ??= new ServiceRepository(_context);
        public IFeedbackRepository Feedbacks => _feedbacks ??= new FeedbackRepository(_context);
        public IRechargePlanRepository RechargePlans => _rechargePlans ??= new RechargePlanRepository(_context);
        public ITransactionRepository Transactions => _transactions ??= new TransactionRepository(_context);
        public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            UpdateAuditFields();
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CommitAsync()
        {
            return await SaveChangesAsync();
            //await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            //hoan tac cac thay doi trong Change Tracker neu co loi
            _context.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Unchanged);
            await Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

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
