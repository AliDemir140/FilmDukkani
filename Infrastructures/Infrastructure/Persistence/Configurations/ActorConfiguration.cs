using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ActorConfiguration : IEntityTypeConfiguration<Actor>
    {
        public void Configure(EntityTypeBuilder<Actor> builder)
        {
            builder.HasKey(a => a.ID);

            builder.Property(a => a.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(a => a.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(a => a.Biography)
                   .HasMaxLength(1000);
        }
    }
}
