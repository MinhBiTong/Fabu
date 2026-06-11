using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class TelecomProductConfiguration : IEntityTypeConfiguration<TelecomProduct>
    {
        public void Configure(EntityTypeBuilder<TelecomProduct> builder)
        {
            builder.ToTable("TelecomProducts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductCode)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.ProductName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Category)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Brand)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.Tags)
                .HasMaxLength(500);

            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.OriginalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.IsPublished)
                .HasDefaultValue(true);

            builder.HasIndex(x => x.ProductCode)
                .IsUnique();

            builder.HasIndex(x => new { x.Category, x.IsActive, x.IsPublished })
                .HasDatabaseName("IX_TelecomProducts_Category_Status");
        }
    }
}
