using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Password)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Phone)
                   .HasMaxLength(20);

            builder.Property(x => x.MembershipStartDate)
                   .IsRequired();

            builder.HasOne(m => m.MembershipPlan)
                   .WithMany(p => p.Members)
                   .HasForeignKey(m => m.MembershipPlanId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
