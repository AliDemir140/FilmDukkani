using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MovieDirectorConfiguration : IEntityTypeConfiguration<MovieDirector>
    {
        public void Configure(EntityTypeBuilder<MovieDirector> builder)
        {
            builder.HasKey(md => md.ID);

            builder.HasOne(md => md.Movie)
                   .WithMany(m => m.MovieDirectors)
                   .HasForeignKey(md => md.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(md => md.Director)
                   .WithMany(d => d.MovieDirectors)
                   .HasForeignKey(md => md.DirectorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
