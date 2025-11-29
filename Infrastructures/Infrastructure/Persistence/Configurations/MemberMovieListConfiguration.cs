using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MemberMovieListConfiguration : IEntityTypeConfiguration<MemberMovieList>
    {
        public void Configure(EntityTypeBuilder<MemberMovieList> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasOne(l => l.Member)
                   .WithMany() // İleride Member içine ICollection<MemberMovieList> eklersek WithMany(m => m.Lists) yaparız
                   .HasForeignKey(l => l.MemberId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(l => l.Items)
                   .WithOne(i => i.MemberMovieList)
                   .HasForeignKey(i => i.MemberMovieListId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
