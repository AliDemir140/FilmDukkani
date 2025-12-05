using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Enums;


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

            builder.Property(dr => dr.Status)
                   .HasDefaultValue(DeliveryStatus.Pending);

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
