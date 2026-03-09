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

            builder.HasOne(x => x.Transaction)
                .WithMany()
                .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.PostpaidBill)
                .WithMany()
                .HasForeignKey(x => x.BillId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
