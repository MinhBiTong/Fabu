
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IFeedbackRepository Feedbacks { get; }
        ICustomerServiceRepository CustomerServices { get; }
        ICustomerRepository Customers { get; }
        IServiceRepository Services { get; }
        IUserRepository Users { get; }
        IRechargePlanRepository RechargePlans { get; }
        IRoleRepository Roles { get; }
        IPermissionRepository Permissions { get; }
        IAccountRepository Accounts { get; }
        IAuditLogRepository AuditLogs { get; }
        ICouponRepository Coupons { get; }
        ICouponUsageRepository CouponUsages { get; }
        IPaymentRepository Payments { get; }
        ITransactionRepository Transactions { get; }
        IPostpaidBillRepository PostpaidBills { get; }
        ITelecomProductRepository TelecomProducts { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        IOrderRepository Orders { get; }
        Task<int> CommitAsync();
        Task RollbackAsync();
        Task<int> CommitAsync(int commitId);
        Task<int> SaveChangesAsync();
        Task<IUnitOfWorkTransaction> BeginTransactionAsync();
        void Dispose();
        ValueTask DisposeAsync();


    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {

    }
}

