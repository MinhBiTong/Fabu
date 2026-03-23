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
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("Coupons");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.DiscountType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(x => x.DiscountValue)
                .HasPrecision(18, 2);

            builder.Property(x => x.MinRechargeAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.MaxDiscount)
                .HasPrecision(18, 2);

            builder.Property(x => x.ValidFrom)
                .IsRequired();
            builder.Property(x => x.ValidTo)
                .IsRequired();
            builder.Property(x => x.UsageLimitPerUser)
                .IsRequired()
                .HasDefaultValue(1);
            builder.Property(x => x.UsageLimitTotal)
                .IsRequired()
                .HasDefaultValue(1000);

            builder.Property(x => x.CreatedByUserId)
                .IsRequired(false);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasOne(x => x.CreatedByUser)
                .WithMany(u => u.Coupons) 
                .HasForeignKey(x => x.CreatedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
