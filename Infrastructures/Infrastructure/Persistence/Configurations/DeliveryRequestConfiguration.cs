using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DeliveryRequestConfiguration : IEntityTypeConfiguration<DeliveryRequest>
    {
        public void Configure(EntityTypeBuilder<DeliveryRequest> builder)
        {
            builder.HasKey(x => x.ID);

            // Member ilişkisi -> burada CASCADE YAPMIYORUZ (multiple cascade path kırmak için)
            builder.HasOne(x => x.Member)
                   .WithMany()
                   .HasForeignKey(x => x.MemberId)
                   .OnDelete(DeleteBehavior.NoAction); // 🔥 KRİTİK

            // List ilişkisi -> CASCADE (liste silinirse request de silinsin)
            builder.HasOne(x => x.MemberMovieList)
                   .WithMany(x => x.DeliveryRequests)
                   .HasForeignKey(x => x.MemberMovieListId)
                   .OnDelete(DeleteBehavior.Cascade); // 🔥 KRİTİK

            builder.Property(x => x.Status).IsRequired();
        }
    }
}
