using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Balance)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.CreditLimit)
                .HasPrecision(18, 2);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(x => x.LastRechargeDate);

            builder.HasIndex(x => x.CustomerId)
                .IsUnique();

            builder.HasOne(x => x.Customer)
                .WithOne(c => c.Account)
                .HasForeignKey<Account>(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
