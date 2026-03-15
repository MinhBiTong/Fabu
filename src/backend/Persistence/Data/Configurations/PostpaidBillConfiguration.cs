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
    public class PostpaidBillConfiguration : IEntityTypeConfiguration<PostpaidBill>
    {
        public void Configure(EntityTypeBuilder<PostpaidBill> builder)
        {
            builder.ToTable("PostpaidBills");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BillMonth)
                .IsRequired();

            builder.Property(x => x.DueDate)
                .IsRequired();

            builder.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.PaidAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.PostpaidBills)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Payments)
                .WithOne(x => x.PostpaidBill)
                .HasForeignKey(x => x.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(b => new { b.CustomerId, b.BillMonth })
                .HasDatabaseName("IX_PostpaidBills_CustomerId_BillMonth")
                .IsUnique();
        }
    }
}
