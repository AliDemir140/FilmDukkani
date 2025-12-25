using Infrastructure.Persistence;
using Infrastructure.Persistence.SeedData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DevToolsController : ControllerBase
    {
        private readonly FilmDukkaniDbContext _context;
        private readonly DatabaseSeeder _seeder;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public DevToolsController(
            FilmDukkaniDbContext context,
            DatabaseSeeder seeder,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _context = context;
            _seeder = seeder;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _env = env;
        }

        // POST: /api/DevTools/reset
        [HttpPost("reset")]
        public async Task<IActionResult> Reset()
        {
            // ✅ Güvenlik: sadece Development ortamında çalışsın
            if (!_env.IsDevelopment())
                return BadRequest("Bu endpoint sadece Development ortamında çalışır.");

            // 1) Domain tablolarını temizle (FK sırasına dikkat)
            await ExecuteDeleteSafeAsync(_context.DeliveryRequestItems);
            await ExecuteDeleteSafeAsync(_context.DeliveryRequests);

            await ExecuteDeleteSafeAsync(_context.MemberMovieListItems);
            await ExecuteDeleteSafeAsync(_context.MemberMovieLists);

            await ExecuteDeleteSafeAsync(_context.DamagedMovies);
            await ExecuteDeleteSafeAsync(_context.MovieCopies);

            await ExecuteDeleteSafeAsync(_context.MovieAwards);
            await ExecuteDeleteSafeAsync(_context.MovieActors);
            await ExecuteDeleteSafeAsync(_context.MovieDirectors);

            await ExecuteDeleteSafeAsync(_context.Awards);
            await ExecuteDeleteSafeAsync(_context.Actors);
            await ExecuteDeleteSafeAsync(_context.Directors);

            await ExecuteDeleteSafeAsync(_context.Movies);
            await ExecuteDeleteSafeAsync(_context.Members);
            await ExecuteDeleteSafeAsync(_context.MembershipPlans);
            await ExecuteDeleteSafeAsync(_context.Categories);

            // 2) Identity kullanıcılarını sil (Admin dahil)
            var users = await _userManager.Users.ToListAsync();
            foreach (var u in users)
                await _userManager.DeleteAsync(u);

            // 3) Rolleri sil (temiz reset için)
            var roles = await _roleManager.Roles.ToListAsync();
            foreach (var r in roles)
                await _roleManager.DeleteAsync(r);

            // 4) Tekrar seed
            await RoleSeed.SeedAsync(_roleManager);
            await _seeder.SeedReferenceDataAsync();
            await _seeder.SeedAdminAsync(_configuration); 
            await _seeder.SeedDevDataAsync();

            return Ok(new
            {
                message = "DEV reset tamamlandı: Reference + Admin + DevData seed edildi.",
                admin = _configuration["SeedAdmin:Email"]
            });
        }

        // ✅ Helper: EF Core 7/8 ExecuteDelete yoksa fallback çalışsın
        private static async Task ExecuteDeleteSafeAsync<TEntity>(DbSet<TEntity> set) where TEntity : class
        {
            try
            {
                await set.ExecuteDeleteAsync(); // EF Core 7+
            }
            catch
            {
                // fallback: memory üzerinden sil
                var list = await set.ToListAsync();
                set.RemoveRange(list);
            }
        }
    }
}
