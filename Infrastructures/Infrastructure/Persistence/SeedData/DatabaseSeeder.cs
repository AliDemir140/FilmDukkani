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

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

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

            if (string.IsNullOrWhiteSpace(member.IdentityUserId))
            {
                member.IdentityUserId = user.Id;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedDevDataAsync()
        {
            Randomizer.Seed = new Random(12345);
            var faker = new Faker("tr");

            await SeedDevMoviesAsync(faker);
            await SeedDevActorsDirectorsAsync(faker);
            await SeedDevMovieCategoriesAsync(faker);
            await SeedDevMovieActorsAsync(faker);
            await SeedDevMovieDirectorsAsync(faker);
        }

        private async Task SeedDevMoviesAsync(Faker faker)
        {
            if (await _context.Movies.AnyAsync())
                return;

            var movieFaker = new Faker<Movie>("tr")
                .RuleFor(m => m.Title, f => f.Lorem.Sentence(3).Replace(".", "").Trim())
                .RuleFor(m => m.OriginalTitle, f => f.Lorem.Sentence(3).Replace(".", "").Trim())
                .RuleFor(m => m.Description, f => f.Lorem.Sentences(2))
                .RuleFor(m => m.ReleaseYear, f => f.Date.Past(25).Year)
                .RuleFor(m => m.TechnicalDetails, f => "DVD")
                .RuleFor(m => m.AudioFeatures, f => "TR/EN")
                .RuleFor(m => m.SubtitleLanguages, f => "TR/EN")
                .RuleFor(m => m.TrailerUrl, f => null)
                .RuleFor(m => m.CoverImageUrl, f => null)
                .RuleFor(m => m.Barcode, f => f.Random.ReplaceNumbers("FDK##########"))
                .RuleFor(m => m.Supplier, f => f.PickRandom(new[] { "Tiglon", "Palermo", "AFM", "Diğer" }))
                .RuleFor(m => m.Status, MovieStatus.Available)
                .RuleFor(m => m.IsEditorsChoice, f => f.Random.Bool(0.20f))
                .RuleFor(m => m.IsNewRelease, f => f.Random.Bool(0.20f))
                .RuleFor(m => m.IsAwardWinner, f => f.Random.Bool(0.15f));

            var movies = movieFaker.Generate(50);

            await _context.Movies.AddRangeAsync(movies);
            await _context.SaveChangesAsync();
        }

        private async Task SeedDevActorsDirectorsAsync(Faker faker)
        {
            if (!await _context.Actors.AnyAsync())
            {
                var actorFaker = new Faker<Actor>("tr")
                    .RuleFor(a => a.FirstName, f => f.Name.FirstName())
                    .RuleFor(a => a.LastName, f => f.Name.LastName())
                    .RuleFor(a => a.Biography, f => null);

                var actors = actorFaker.Generate(60);
                await _context.Actors.AddRangeAsync(actors);
                await _context.SaveChangesAsync();
            }

            if (!await _context.Directors.AnyAsync())
            {
                var directorFaker = new Faker<Director>("tr")
                    .RuleFor(d => d.FirstName, f => f.Name.FirstName())
                    .RuleFor(d => d.LastName, f => f.Name.LastName())
                    .RuleFor(d => d.Biography, f => null);

                var directors = directorFaker.Generate(25);
                await _context.Directors.AddRangeAsync(directors);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedDevMovieCategoriesAsync(Faker faker)
        {
            var movies = await _context.Movies.AsNoTracking().Select(m => m.ID).ToListAsync();
            if (movies.Count == 0)
                return;

            var categories = await _context.Categories.AsNoTracking().Select(c => c.ID).ToListAsync();
            if (categories.Count == 0)
                return;

            foreach (var movieId in movies)
            {
                var exists = await _context.MovieCategories.AnyAsync(x => x.MovieId == movieId);
                if (exists)
                    continue;

                var pickCount = faker.Random.Int(1, Math.Min(3, categories.Count));
                var selected = categories
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(pickCount)
                    .Distinct()
                    .ToList();

                var links = selected.Select(cid => new MovieCategory
                {
                    MovieId = movieId,
                    CategoryId = cid
                });

                await _context.MovieCategories.AddRangeAsync(links);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedDevMovieActorsAsync(Faker faker)
        {
            var movieIds = await _context.Movies.AsNoTracking().Select(m => m.ID).ToListAsync();
            if (movieIds.Count == 0)
                return;

            var actorIds = await _context.Actors.AsNoTracking().Select(a => a.ID).ToListAsync();
            if (actorIds.Count == 0)
                return;

            foreach (var movieId in movieIds)
            {
                var exists = await _context.MovieActors.AnyAsync(x => x.MovieId == movieId);
                if (exists)
                    continue;

                var pickCount = faker.Random.Int(2, 5);
                var selectedActors = actorIds
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(Math.Min(pickCount, actorIds.Count))
                    .ToList();

                var links = selectedActors.Select(aid => new MovieActor
                {
                    MovieId = movieId,
                    ActorId = aid
                });

                await _context.MovieActors.AddRangeAsync(links);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedDevMovieDirectorsAsync(Faker faker)
        {
            var movieIds = await _context.Movies.AsNoTracking().Select(m => m.ID).ToListAsync();
            if (movieIds.Count == 0)
                return;

            var directorIds = await _context.Directors.AsNoTracking().Select(d => d.ID).ToListAsync();
            if (directorIds.Count == 0)
                return;

            foreach (var movieId in movieIds)
            {
                var exists = await _context.MovieDirectors.AnyAsync(x => x.MovieId == movieId);
                if (exists)
                    continue;

                var pickCount = 1;
                var selectedDirectors = directorIds
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(Math.Min(pickCount, directorIds.Count))
                    .ToList();

                var links = selectedDirectors.Select(did => new MovieDirector
                {
                    MovieId = movieId,
                    DirectorId = did
                });

                await _context.MovieDirectors.AddRangeAsync(links);
            }

            await _context.SaveChangesAsync();
        }
    }
}
