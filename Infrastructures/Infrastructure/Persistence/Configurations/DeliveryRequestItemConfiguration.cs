using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DeliveryRequestItemConfiguration : IEntityTypeConfiguration<DeliveryRequestItem>
    {
        public void Configure(EntityTypeBuilder<DeliveryRequestItem> builder)
        {
            builder.HasKey(dri => dri.ID);

            builder.Property(dri => dri.IsReturned)
                   .HasDefaultValue(false);

            builder.Property(dri => dri.IsDamaged)
                   .HasDefaultValue(false);

            builder.HasOne(dri => dri.DeliveryRequest)
                   .WithMany(dr => dr.Items)
                   .HasForeignKey(dri => dri.DeliveryRequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dri => dri.Movie)
                   .WithMany(m => m.DeliveryRequestItems)
                   .HasForeignKey(dri => dri.MovieId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dri => dri.MemberMovieListItem)
                   .WithMany()
                   .HasForeignKey(dri => dri.MemberMovieListItemId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
