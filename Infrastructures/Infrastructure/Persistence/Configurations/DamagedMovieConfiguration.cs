using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DamagedMovieConfiguration : IEntityTypeConfiguration<DamagedMovie>
    {
        public void Configure(EntityTypeBuilder<DamagedMovie> builder)
        {
            builder.HasKey(dm => dm.ID);

            builder.Property(dm => dm.Note)
                   .HasMaxLength(500);

            builder.Property(dm => dm.IsSentToPurchase)
                   .HasDefaultValue(false);

            builder.HasOne(dm => dm.MovieCopy)
                   .WithMany(mc => mc.DamagedMovies)
                   .HasForeignKey(dm => dm.MovieCopyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
