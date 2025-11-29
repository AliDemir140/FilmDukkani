using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MemberMovieListItemConfiguration : IEntityTypeConfiguration<MemberMovieListItem>
    {
        public void Configure(EntityTypeBuilder<MemberMovieListItem> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.Priority)
                   .IsRequired();

            builder.Property(x => x.AddedDate)
                   .IsRequired();

            builder.HasOne(i => i.MemberMovieList)
                   .WithMany(l => l.Items)
                   .HasForeignKey(i => i.MemberMovieListId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Movie)
                   .WithMany() // İleride Movie içine ICollection<MemberMovieListItem> eklemek istersek WithMany(m => m.MemberMovieListItems) yaparız
                   .HasForeignKey(i => i.MovieId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
