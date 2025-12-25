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

        // -------------------------------------------------------------
        // 1) SABİT REFERANS DATA (PROD + DEV)
        // -------------------------------------------------------------
        public async Task SeedReferenceDataAsync()
        {
            // Eğer Category varsa daha önce seed edilmiş demektir
            if (await _context.Categories.AnyAsync())
                return;

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

        // -------------------------------------------------------------
        // 1.5) ADMIN SEED (DEV)
        // -------------------------------------------------------------
        public async Task SeedAdminAsync(IConfiguration configuration)
        {
            var adminCfg = configuration.GetSection("SeedAdmin");
            var email = adminCfg["Email"];
            var password = adminCfg["Password"];
            var firstName = adminCfg["FirstName"] ?? "System";
            var lastName = adminCfg["LastName"] ?? "Admin";

            // Config yoksa sessizce çık
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            // Identity user var mı?
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
                {
                    // İstersen Console.WriteLine ile yazdırabilirsin
                    return;
                }
            }

            // Admin rolüne ekle (RoleSeed zaten rolü oluşturuyor)
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

            // Member kaydı var mı? (IdentityUserId üzerinden)
            var memberExists = await _context.Members.AnyAsync(m => m.IdentityUserId == user.Id);
            if (!memberExists)
            {
                // SeedReferenceDataAsync çalıştıysa planlar vardır; yine de garanti
                int defaultPlanId = await _context.MembershipPlans.Select(x => x.ID).FirstOrDefaultAsync();
                if (defaultPlanId == 0) defaultPlanId = 1;

                var member = new Member
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    IdentityUserId = user.Id,
                    MembershipPlanId = defaultPlanId,
                    MembershipStartDate = DateTime.Today,
                    Status = MemberStatus.Active
                };

                await _context.Members.AddAsync(member);
                await _context.SaveChangesAsync();
            }
        }

        // -------------------------------------------------------------
        // 2) DEVELOPMENT FAKE DATA (Movie, Member, List, Delivery)
        // -------------------------------------------------------------
        public async Task SeedDevDataAsync()
        {
            // Eğer Movie zaten varsa, dev seed yapılmış demektir
            if (await _context.Movies.AnyAsync())
                return;

            Randomizer.Seed = new Random(12345);
            var faker = new Faker("tr");

            var categories = await _context.Categories.ToListAsync();
            var plans = await _context.MembershipPlans.ToListAsync();

            // MOVIES (50 adet)
            var movieFaker = new Faker<Movie>("tr")
                .RuleFor(m => m.Title, f => f.Lorem.Sentence(3))
                .RuleFor(m => m.Description, f => f.Lorem.Sentences(2))
                .RuleFor(m => m.ReleaseYear, f => f.Date.Past(20).Year)
                .RuleFor(m => m.CategoryId, f => f.PickRandom(categories).ID)
                .RuleFor(m => m.Status, MovieStatus.Available);

            var movies = movieFaker.Generate(50);

            await _context.Movies.AddRangeAsync(movies);
            await _context.SaveChangesAsync();

            //// MEMBERS (20 adet)
            //var memberFaker = new Faker<Member>("tr")
            //    .RuleFor(m => m.FirstName, f => f.Name.FirstName())
            //    .RuleFor(m => m.LastName, f => f.Name.LastName())
            //    .RuleFor(m => m.Email, (f, m) => f.Internet.Email(m.FirstName, m.LastName))
            //    .RuleFor(m => m.Password, f => "123456")
            //    .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber("05#########"))
            //    .RuleFor(m => m.MembershipPlanId, f => f.PickRandom(plans).ID)
            //    .RuleFor(m => m.MembershipStartDate, f => f.Date.Past(1))
            //    .RuleFor(m => m.Status, MemberStatus.Active);

            //var members = memberFaker.Generate(20);

            //await _context.Members.AddRangeAsync(members);
            //await _context.SaveChangesAsync();

            // MEMBER MOVIE LIST (her üyeye 1 liste)
            //var lists = new List<MemberMovieList>();

            //foreach (var member in members)
            //{
            //    lists.Add(new MemberMovieList
            //    {
            //        MemberId = member.ID,
            //        Name = $"{member.FirstName} {member.LastName} - Liste"
            //    });
            //}

            //await _context.MemberMovieLists.AddRangeAsync(lists);
            //await _context.SaveChangesAsync();

            //// LIST ITEMS (her listeye 10 film)
            //var listItems = new List<MemberMovieListItem>();

            //foreach (var list in lists)
            //{
            //    var randomMovies = movies.OrderBy(m => Guid.NewGuid()).Take(10).ToList();
            //    int priority = 1;

            //    foreach (var movie in randomMovies)
            //    {
            //        listItems.Add(new MemberMovieListItem
            //        {
            //            MemberMovieListId = list.ID,
            //            MovieId = movie.ID,
            //            Priority = priority++,
            //            AddedDate = faker.Date.Past(1)
            //        });
            //    }
            //}

            //await _context.MemberMovieListItems.AddRangeAsync(listItems);
            //await _context.SaveChangesAsync();

            //// DELIVERY REQUEST (İlk 5 üye)
            //var tomorrow = DateTime.Today.AddDays(1);
            //var requests = new List<DeliveryRequest>();

            //foreach (var member in members.Take(5))
            //{
            //    var list = lists.First(l => l.MemberId == member.ID);

            //    requests.Add(new DeliveryRequest
            //    {
            //        MemberId = member.ID,
            //        MemberMovieListId = list.ID,
            //        RequestedDate = DateTime.Now.AddDays(-1),
            //        DeliveryDate = tomorrow,
            //        Status = DeliveryStatus.Pending
            //    });
            //}

            //await _context.DeliveryRequests.AddRangeAsync(requests);
            //await _context.SaveChangesAsync();
        }
    }
}
