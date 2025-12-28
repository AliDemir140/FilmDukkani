using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BillingAttemptConfiguration : IEntityTypeConfiguration<BillingAttempt>
    {
        public void Configure(EntityTypeBuilder<BillingAttempt> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.MemberId).IsRequired();

            builder.Property(x => x.Period)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.Amount)
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(x => x.Success)
                   .IsRequired();

            builder.Property(x => x.Error)
                   .HasMaxLength(500);

            builder.Property(x => x.AttemptedAt)
                   .IsRequired();

            builder.HasOne(x => x.Member)
                   .WithMany(m => m.BillingAttempts)
                   .HasForeignKey(x => x.MemberId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.MemberId, x.Period });
        }
    }
}
