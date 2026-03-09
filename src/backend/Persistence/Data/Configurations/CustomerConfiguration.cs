using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MobileNumber)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(x => x.CustomerType)
                .HasMaxLength(20);

            builder.Property(x => x.FullName)
                .HasMaxLength(200);

            builder.Property(x => x.Address)
                .HasMaxLength(500);

            builder.HasIndex(x => x.MobileNumber)
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            //builder.HasOne(c => c.Account)
            //        .WithOne(a => a.Customer)
            //        .HasForeignKey<Account>(a => a.CustomerId)
            //        .OnDelete(DeleteBehavior.Cascade);
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
