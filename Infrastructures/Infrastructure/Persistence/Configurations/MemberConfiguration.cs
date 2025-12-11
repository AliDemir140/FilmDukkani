using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.HasKey(m => m.ID);

            builder.Property(m => m.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(m => m.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(m => m.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.Password)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(m => m.Phone)
                   .HasMaxLength(20);

            builder.Property(m => m.MembershipStartDate)
                   .IsRequired();

            // 🔹 ENUM mapping
            builder.Property(m => m.Status)
                   .HasConversion<byte>()                  // DB’de tinyint
                   .HasDefaultValue(MemberStatus.Active)   // varsayılan
                   .IsRequired();

            builder.HasOne(m => m.MembershipPlan)
                   .WithMany(p => p.Members)
                   .HasForeignKey(m => m.MembershipPlanId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
