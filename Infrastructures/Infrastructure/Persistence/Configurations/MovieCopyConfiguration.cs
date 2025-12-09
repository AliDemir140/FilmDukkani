using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MovieCopyConfiguration : IEntityTypeConfiguration<MovieCopy>
    {
        public void Configure(EntityTypeBuilder<MovieCopy> builder)
        {
            builder.HasKey(mc => mc.ID);

            builder.Property(mc => mc.Barcode)
                   .IsRequired()
                   .HasMaxLength(50);

            // Barkod benzersiz olsun
            builder.HasIndex(mc => mc.Barcode)
                   .IsUnique();

            builder.Property(mc => mc.IsAvailable)
                   .HasDefaultValue(true);

            builder.Property(mc => mc.IsDamaged)
                   .HasDefaultValue(false);

            builder.HasOne(mc => mc.Movie)
                   .WithMany(m => m.MovieCopies)
                   .HasForeignKey(mc => mc.MovieId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mc => mc.Shelf)
                   .WithMany(s => s.MovieCopies)
                   .HasForeignKey(mc => mc.ShelfId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
