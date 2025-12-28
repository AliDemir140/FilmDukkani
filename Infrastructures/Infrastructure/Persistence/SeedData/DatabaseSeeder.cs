using Bogus;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.SeedData
{
    public class DatabaseSeeder
    {
        private readonly FilmDukkaniDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DatabaseSeeder(FilmDukkaniDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedReferenceDataAsync()
        {
            if (!await _context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { CategoryName = "Aksiyon" },
                    new Category { CategoryName = "Dram" },
                    new Category { CategoryName = "Komedi" },
                    new Category { CategoryName = "Bilim Kurgu" },
                    new Category { CategoryName = "Korku" },
                    new Category { CategoryName = "Animasyon" },
                    new Category { CategoryName = "Belgesel" },
                    new Category { CategoryName = "Gerilim" }
                };

                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
            }

            if (!await _context.MembershipPlans.AnyAsync())
            {
                var plans = new List<MembershipPlan>
                {
                    new MembershipPlan
                    {
                        PlanName = "Ekonomik Paket",
                        Price = 19.90m,
                        MaxMoviesPerMonth = 16,
                        MaxChangePerMonth = 8,
                        Description = "Bir değişimde 2 film"
                    },
                    new MembershipPlan
                    {
                        PlanName = "3'lü Paket",
                        Price = 29.90m,
                        MaxMoviesPerMonth = 24,
                        MaxChangePerMonth = 8,
                        Description = "Bir değişimde 3 film"
                    },
                    new MembershipPlan
                    {
                        PlanName = "4'lü Paket",
                        Price = 39.90m,
                        MaxMoviesPerMonth = 32,
                        MaxChangePerMonth = 8,
                        Description = "Bir değişimde 4 film"
                    },
                    new MembershipPlan
                    {
                        PlanName = "Gold Paket",
                        Price = 49.90m,
                        MaxMoviesPerMonth = 40,
                        MaxChangePerMonth = 10,
                        Description = "Bir değişimde 5 film"
                    }
                };

                await _context.MembershipPlans.AddRangeAsync(plans);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedAdminAsync(IConfiguration configuration)
        {
            var adminCfg = configuration.GetSection("SeedAdmin");
            var email = adminCfg["Email"];
            var password = adminCfg["Password"];
            var firstName = adminCfg["FirstName"] ?? "System";
            var lastName = adminCfg["LastName"] ?? "Admin";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createRes = await _userManager.CreateAsync(user, password);
                if (!createRes.Succeeded)
                    throw new InvalidOperationException(string.Join(", ", createRes.Errors.Select(x => x.Description)));
            }

            // Role ekleme için RoleManager zaten seeded olmalı
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

            // Member var mı?
            var member = await _context.Members.FirstOrDefaultAsync(m => m.IdentityUserId == user.Id);

            if (member == null)
            {
                int defaultPlanId = await _context.MembershipPlans.Select(x => x.ID).FirstOrDefaultAsync();
                if (defaultPlanId == 0) defaultPlanId = 1;

                member = new Member
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    IdentityUserId = user.Id,

                    Phone = "-",
                    AddressLine = "-",
                    City = "-",
                    District = "-",
                    PostalCode = "-",

                    ContractAccepted = true,
                    ContractAcceptedAt = DateTime.UtcNow,
                    ContractVersion = "v1",

                    MembershipPlanId = defaultPlanId,
                    MembershipStartDate = DateTime.Today,
                    Status = MemberStatus.Active
                };

                await _context.Members.AddAsync(member);
                await _context.SaveChangesAsync();
                return;
            }

            // Member varsa ama IdentityUserId boş kalmışsa düzelt
            if (string.IsNullOrWhiteSpace(member.IdentityUserId))
            {
                member.IdentityUserId = user.Id;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedDevDataAsync()
        {
            if (await _context.Movies.AnyAsync())
                return;

            Randomizer.Seed = new Random(12345);
            var faker = new Faker("tr");

            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            if (categories.Count == 0)
                return;

            var movieFaker = new Faker<Movie>("tr")
                .RuleFor(m => m.Title, f => f.Lorem.Sentence(3))
                .RuleFor(m => m.Description, f => f.Lorem.Sentences(2))
                .RuleFor(m => m.ReleaseYear, f => f.Date.Past(20).Year)
                .RuleFor(m => m.Status, MovieStatus.Available)
                .RuleFor(m => m.IsEditorsChoice, f => f.Random.Bool(0.20f))
                .RuleFor(m => m.IsNewRelease, f => f.Random.Bool(0.20f))
                .RuleFor(m => m.IsAwardWinner, f => f.Random.Bool(0.15f));

            var movies = movieFaker.Generate(50);

            await _context.Movies.AddRangeAsync(movies);
            await _context.SaveChangesAsync();

            var movieCategories = new List<MovieCategory>();

            foreach (var movie in movies)
            {
                var pickCount = faker.Random.Int(1, Math.Min(3, categories.Count));
                var selected = categories
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(pickCount)
                    .Select(x => x.ID)
                    .Distinct()
                    .ToList();

                foreach (var cid in selected)
                {
                    movieCategories.Add(new MovieCategory
                    {
                        MovieId = movie.ID,
                        CategoryId = cid
                    });
                }
            }

            _context.Set<MovieCategory>().AddRange(movieCategories);
            await _context.SaveChangesAsync();
        }
    }
}
