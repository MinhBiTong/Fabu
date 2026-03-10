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
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ServiceName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.ServiceCode)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.ServiceCode)
                .IsUnique();

            builder.Property(x => x.Category)
                .HasMaxLength(100);

            builder.Property(x => x.DataAmountMB)
                .IsRequired();

            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.IsAutoRenew)
                .HasDefaultValue(0);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);
        }
    }
}
