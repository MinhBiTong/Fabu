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
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TransactionType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.TransactionRef)
                .HasMaxLength(200);

            builder.Property(x => x.PaymentMethod)
                .HasConversion<int>();

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.Property(x => x.CompletedAt);

            // Quan hệ với Customer (N - 1)
            builder.HasOne(x => x.Customer)
                .WithMany(c => c.Transactions)
                .HasForeignKey(x => x.CustomerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với Payment (N - 1)
            // Một Payment có thể có nhiều Transaction (thử lại, hoàn tiền...)
            builder.HasOne(x => x.Payment)
                .WithMany(p => p.Transactions)
                .HasForeignKey(x => x.PaymentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction); // Tránh vòng lặp Cascade xóa

            builder.HasIndex(t => new { t.CustomerId, t.CreatedDate})
                .HasDatabaseName("IX_Transactions_CustomerId_CreatedDate")
                .IsDescending(new[] { false, true }); // Sắp xếp theo CreatedDate giảm dần

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_Transactions_Status");
        }
    }
}
