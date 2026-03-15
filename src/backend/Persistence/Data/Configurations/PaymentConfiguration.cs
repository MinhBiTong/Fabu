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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.PaymentDate)
                .IsRequired();

            builder.Property(x => x.PaymentRef)
                .HasMaxLength(200);

            builder.Property(x => x.PaymentMethod)
                .HasConversion<int>();

            builder.Property(x => x.Status)
                .HasConversion<int>();

            // Quan hệ với PostpaidBill (N - 1 hoặc 1 - 1 tùy logic Bill của bạn)
            builder.HasOne(x => x.PostpaidBill)
                .WithMany() // Giả định một Bill có thể có nhiều đợt Payment (trả góp/từng phần)
                .HasForeignKey(x => x.BillId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.Payment)
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.NoAction); 
        }
    }
}
