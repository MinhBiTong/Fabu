
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
        Task<int> CommitAsync();
        Task RollbackAsync();
        Task<int> CommitAsync(int commitId);
        Task<int> SaveChangesAsync();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {

    }
}

