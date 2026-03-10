using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Abstractions.Entities;
using System.Linq.Expressions;
using Domain.Abstractions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Persistence.Data.Contexts
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {
        private readonly IUserContext _userContext;
        public AppDbContext(DbContextOptions<AppDbContext> options, IUserContext userContext) : base(options)
        {
            _userContext = userContext;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CouponUsage> CouponUsages { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<PostpaidBill> PostpaidBills { get; set; }
        public DbSet<RechargePlan> RechargePlans { get; set; }
        public DbSet<CustomerService> CustomerServices { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);            
            // Global query filter for soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(EF).GetMethod(nameof(EF.Property), new[] { typeof(object), typeof(string) })
                        ?.MakeGenericMethod(typeof(bool));

                    if (method != null)
                    {
                        var parameter = Expression.Parameter(entityType.ClrType, "e");

                        var propertyMethod = typeof(EF)
                            .GetMethod(nameof(EF.Property))
                            .MakeGenericMethod(typeof(bool));

                        var isDeletedProperty = Expression.Call(
                            propertyMethod,
                            parameter,
                            Expression.Constant("IsDeleted")
                        );

                        var compareExpression = Expression.Equal(isDeletedProperty, Expression.Constant(false));

                        var lambda = Expression.Lambda(compareExpression, parameter);

                        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //auto set audit, lay user tu HttpContext
            var now = DateTime.UtcNow;

            //lay username tu claims trong jwt token, neu ko co thi mac dinh la System, sau nay co the thay bang IHttpContextAccessor de lay username tu claims cua token
            var username = _userContext.UserName ?? "System"; //thay bang IHttpContextAccessor
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                // 1. Xử lý Date Tracking
                if (entry.Entity is IDateTracking dateTracking)
                {
                    if (entry.State == EntityState.Added)
                        dateTracking.CreatedDate = now;
                    else if (entry.State == EntityState.Modified)
                        dateTracking.ModifiedDate = now;
                }

                // 2. Xử lý User Tracking
                if (entry.Entity is IUserTracking userTracking)
                {
                    if (entry.State == EntityState.Added)
                        userTracking.CreatedBy = username;
                    else if (entry.State == EntityState.Modified)
                        userTracking.ModifiedBy = username;
                }

                // 3. Xử lý Soft Delete (Xóa mềm)
                if (entry.Entity is ISoftDelete softDelete && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified; // Chuyển từ Xóa sang Sửa
                    softDelete.IsDeleted = true;
                    softDelete.DeletedAt = now;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
