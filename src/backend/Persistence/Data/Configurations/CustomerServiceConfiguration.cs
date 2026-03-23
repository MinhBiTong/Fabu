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
    public class CustomerServiceConfiguration : IEntityTypeConfiguration<CustomerService>
    {
        public void Configure(EntityTypeBuilder<CustomerService> builder)
        {
            builder.ToTable("CustomerServices");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActivatedAt)
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.IsAutoRenewed)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerServices)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Service)
                .WithMany(x => x.CustomerServices)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(cs => new { cs.CustomerId, cs.IsAutoRenewed })
                .HasDatabaseName("IX_CustomerServices_CustomerId_IsAutoRenewed");

            builder.HasIndex(cs => cs.ExpiresAt)
                .HasDatabaseName("IX_CustomerServices_ExpiresAt");
        }
    }
}
