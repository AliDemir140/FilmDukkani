using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DeliveryRequestConfiguration : IEntityTypeConfiguration<DeliveryRequest>
    {
        public void Configure(EntityTypeBuilder<DeliveryRequest> builder)
        {
            builder.HasKey(dr => dr.ID);

            builder.Property(dr => dr.RequestedDate)
                   .IsRequired();

            builder.Property(dr => dr.DeliveryDate)
                   .IsRequired();

            // Şimdilik Status küçük bir byte olarak kullanılacak
            builder.Property(dr => dr.Status)
                   .HasDefaultValue((byte)0); // Pending

            builder.HasOne(dr => dr.Member)
                   .WithMany(m => m.DeliveryRequests)
                   .HasForeignKey(dr => dr.MemberId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dr => dr.MemberMovieList)
                   .WithMany(l => l.DeliveryRequests)
                   .HasForeignKey(dr => dr.MemberMovieListId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
