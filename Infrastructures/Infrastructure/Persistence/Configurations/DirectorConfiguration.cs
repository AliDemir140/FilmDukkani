using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DirectorConfiguration : IEntityTypeConfiguration<Director>
    {
        public void Configure(EntityTypeBuilder<Director> builder)
        {
            builder.HasKey(d => d.ID);

            builder.Property(d => d.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.Biography)
                   .HasMaxLength(1000);
        }
    }
}
