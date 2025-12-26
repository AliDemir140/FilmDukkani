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

            // Member ilişkisi (navigation bağlı) + NO ACTION (multiple cascade path kırmak için)
            builder.HasOne(x => x.Member)
                   .WithMany(m => m.DeliveryRequests)
                   .HasForeignKey(x => x.MemberId)
                   .OnDelete(DeleteBehavior.NoAction);

            // List ilişkisi
            builder.HasOne(x => x.MemberMovieList)
                   .WithMany(x => x.DeliveryRequests)
                   .HasForeignKey(x => x.MemberMovieListId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Status).IsRequired();

            // Cancel alanları
            builder.Property(x => x.CancelReason)
                   .HasMaxLength(500)
                   .IsUnicode(true)
                   .IsRequired(false);

            builder.Property(x => x.CancelRequestedAt)
                   .IsRequired(false);

            builder.Property(x => x.CancelDecisionAt)
                   .IsRequired(false);

            builder.Property(x => x.CancelApproved)
                   .IsRequired(false);

            builder.Property(x => x.CancelPreviousStatus)
                   .IsRequired(false);

            // DB KURALI: Aynı liste için aynı anda sadece 1 aktif sipariş olabilir
            // Active = Pending(0), Prepared(1), Shipped(2), Delivered(3)
            builder.HasIndex(x => x.MemberMovieListId)
                   .IsUnique()
                   .HasDatabaseName("UX_DeliveryRequests_ActivePerList")
                   .HasFilter("[Status] IN (0,1,2,3)");
        }
    }
}
