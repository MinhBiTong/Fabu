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
    public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> builder)
        {
            builder.ToTable("CouponUsages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DiscountApplied)
                .HasPrecision(18, 2);

            builder.Property(x => x.Status)
                .HasMaxLength(50);

            // FK rõ ràng, không để EF tự sinh 1
            builder.HasOne(cu => cu.Coupon)
                   .WithMany(c => c.CouponUsages)
                   .HasForeignKey(cu => cu.CouponId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cu => cu.Transaction)
                   .WithMany(t => t.CouponUsages) 
                   .HasForeignKey(cu => cu.TransactionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
