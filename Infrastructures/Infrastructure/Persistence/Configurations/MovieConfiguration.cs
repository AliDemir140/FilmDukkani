using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(m => m.ID);

            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.OriginalTitle)
                .HasMaxLength(200);

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            builder.Property(m => m.ReleaseYear)
                .IsRequired();

            builder.Property(m => m.TechnicalDetails)
                .HasMaxLength(500);

            builder.Property(m => m.AudioFeatures)
                .HasMaxLength(300);

            builder.Property(m => m.SubtitleLanguages)
                .HasMaxLength(300);

            builder.Property(m => m.TrailerUrl)
                .HasMaxLength(500);

            builder.Property(m => m.CoverImageUrl)
                .HasMaxLength(500);

            builder.Property(m => m.Barcode)
                .HasMaxLength(50);

            builder.Property(m => m.Supplier)
                .HasMaxLength(100);

            builder.Property(m => m.Status)
                   .HasConversion<byte>()
                   .HasDefaultValue(MovieStatus.Available)
                   .IsRequired();

            builder.Property(m => m.IsEditorsChoice)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(m => m.IsNewRelease)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.HasIndex(m => m.IsEditorsChoice);
            builder.HasIndex(m => m.IsNewRelease);
        }
    }
}
