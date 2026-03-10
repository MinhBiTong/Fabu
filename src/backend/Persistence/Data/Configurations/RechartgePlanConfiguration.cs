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
    public class RechartgePlanConfiguration : IEntityTypeConfiguration<RechargePlan>
    {
        public void Configure(EntityTypeBuilder<RechargePlan> builder)
        {
            builder.ToTable("RechargePlans");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PlanName)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.BonusAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);
        }
    }
}
