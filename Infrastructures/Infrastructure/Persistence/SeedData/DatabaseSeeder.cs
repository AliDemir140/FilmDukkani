using Bogus;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.SeedData
{
    public static class DatabaseSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            Randomizer.Seed = new Random(12345);

            // 1. Kategoriler
            var categories = GenerateCategories();
            modelBuilder.Entity<Category>().HasData(categories);
        }

        /// <summary>
        /// 10 kategori oluştur
        /// </summary>
        private static List<Category> GenerateCategories()
        {
            var categoryNames = new[]
            {
                "Aksiyon",
                "Macera",
                "Dram",
                "Korku",
                "Bilim Kurgu",
                "Komedi",
                "Gerilim",
                "Fantastik",
                "Belgesel",
                "Animasyon"
            };

            var categories = new List<Category>();
            var faker = new Faker("tr");

            for (int i = 0; i < categoryNames.Length; i++)
            {
                categories.Add(new Category
                {
                    ID = i + 1,
                    CategoryName = categoryNames[i],
                    CreatedDate = faker.Date.Past(2),
                    ModifiedDate = DateTime.Now
                });
            }

            return categories;
        }
    }
}
