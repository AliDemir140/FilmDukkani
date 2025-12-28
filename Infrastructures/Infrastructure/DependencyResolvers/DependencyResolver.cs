using Application.Interfaces;
using Application.Repositories;
using Application.ServiceManager;
using Infrastructure.Persistence;
using Infrastructure.Persistence.SeedData;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyResolvers
{
    public static class DependencyResolver
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FilmDukkaniDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<FilmDukkaniDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

            services.AddScoped<DatabaseSeeder>();

            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IMembershipPlanRepository, MembershipPlanRepository>();
            services.AddScoped<IMemberMovieListRepository, MemberMovieListRepository>();
            services.AddScoped<IMemberMovieListItemRepository, MemberMovieListItemRepository>();
            services.AddScoped<IDeliveryRequestRepository, DeliveryRequestRepository>();
            services.AddScoped<IDeliveryRequestItemRepository, DeliveryRequestItemRepository>();
            services.AddScoped<IActorRepository, ActorRepository>();
            services.AddScoped<IDirectorRepository, DirectorRepository>();
            services.AddScoped<IAwardRepository, AwardRepository>();
            services.AddScoped<IShelfRepository, ShelfRepository>();
            services.AddScoped<IMovieCopyRepository, MovieCopyRepository>();
            services.AddScoped<IDamagedMovieRepository, DamagedMovieRepository>();
            services.AddScoped<IPurchaseRequestRepository, PurchaseRequestRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

            services.AddScoped<CategoryServiceManager>();
            services.AddScoped<MovieServiceManager>();
            services.AddScoped<MemberServiceManager>();
            services.AddScoped<MembershipPlanServiceManager>();
            services.AddScoped<MemberMovieListServiceManager>();
            services.AddScoped<DeliveryRequestServiceManager>();
            services.AddScoped<ActorServiceManager>();
            services.AddScoped<DirectorServiceManager>();
            services.AddScoped<AwardServiceManager>();
            services.AddScoped<AccountingServiceManager>();
            services.AddScoped<ShelfServiceManager>();
            services.AddScoped<MovieCopyServiceManager>();
            services.AddScoped<DamagedMovieServiceManager>();
            services.AddScoped<WarehouseServiceManager>();
            services.AddScoped<AccountingReportServiceManager>();
            services.AddScoped<PurchaseRequestServiceManager>();
            services.AddScoped<ReviewServiceManager>();

            services.AddScoped<UserServiceManager>();

            services.AddScoped<IEmailService, FakeEmailService>();
            services.AddScoped<ISmsService, FakeSmsService>();
        }
    }
}
