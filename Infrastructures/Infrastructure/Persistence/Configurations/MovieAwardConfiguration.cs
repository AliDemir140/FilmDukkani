using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MovieAwardConfiguration : IEntityTypeConfiguration<MovieAward>
    {
        public void Configure(EntityTypeBuilder<MovieAward> builder)
        {
            builder.HasKey(ma => ma.ID);

            builder.Property(ma => ma.Category)
                   .HasMaxLength(200);

            builder.HasOne(ma => ma.Movie)
                   .WithMany(m => m.MovieAwards)
                   .HasForeignKey(ma => ma.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ma => ma.Award)
                   .WithMany(a => a.MovieAwards)
                   .HasForeignKey(ma => ma.AwardId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ma => ma.IsWinner)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(ma => ma.Year)
                   .IsRequired(false);

            // Aynı film, aynı ödül, aynı yıl kombinasyonu tekil olsun
            builder.HasIndex(ma => new { ma.MovieId, ma.AwardId, ma.Year })
                   .IsUnique(false);
        }
    }
}
