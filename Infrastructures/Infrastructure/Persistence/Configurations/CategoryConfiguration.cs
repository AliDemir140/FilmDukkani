using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.CategoryName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(c => c.Movies)
                   .WithOne(m => m.Category)
                   .HasForeignKey(m => m.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
