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

            builder.HasIndex(x => new { x.MemberId, x.Name })
                   .IsUnique();

            builder.HasOne(l => l.Member)
                   .WithMany()
                   .HasForeignKey(l => l.MemberId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(l => l.Items)
                   .WithOne(i => i.MemberMovieList)
                   .HasForeignKey(i => i.MemberMovieListId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(l => l.DeliveryRequests)
                   .WithOne(r => r.MemberMovieList)
                   .HasForeignKey(r => r.MemberMovieListId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
