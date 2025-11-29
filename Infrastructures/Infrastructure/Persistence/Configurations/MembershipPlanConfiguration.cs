using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MembershipPlanConfiguration : IEntityTypeConfiguration<MembershipPlan>
    {
        public void Configure(EntityTypeBuilder<MembershipPlan> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.PlanName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Price)
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(x => x.MaxMoviesPerMonth)
                   .IsRequired();

            builder.Property(x => x.MaxChangePerMonth)
                   .IsRequired();

            builder.Property(x => x.Description)
                   .HasMaxLength(500);

            builder.HasMany(p => p.Members)
                   .WithOne(m => m.MembershipPlan)
                   .HasForeignKey(m => m.MembershipPlanId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
