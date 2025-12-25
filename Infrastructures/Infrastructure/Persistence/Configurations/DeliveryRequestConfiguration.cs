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

            // ✅ Member ilişkisi (navigation'ı bağladık -> shadow FK oluşmaz)
            builder.HasOne(x => x.Member)
                   .WithMany(m => m.DeliveryRequests)
                   .HasForeignKey(x => x.MemberId)
                   .OnDelete(DeleteBehavior.NoAction);

            // ✅ List ilişkisi
            builder.HasOne(x => x.MemberMovieList)
                   .WithMany(x => x.DeliveryRequests)
                   .HasForeignKey(x => x.MemberMovieListId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Status).IsRequired();
        }
    }
}
