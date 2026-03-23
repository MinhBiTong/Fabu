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
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedbacks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Subject)
                .HasMaxLength(200);

            builder.Property(x => x.Message)
                .HasMaxLength(1000);

            builder.Property(x => x.Rating)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.HasOne(x => x.Customer)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(x => x.CustomerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
