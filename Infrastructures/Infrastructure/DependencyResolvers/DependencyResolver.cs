using Application.Repositories;
using Application.ServiceManager;
using Infrastructure.Persistence;
using Infrastructure.Persistence.SeedData;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyResolvers
{
    public static class DependencyResolver
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            // DATABASE
            services.AddDbContext<FilmDukkaniDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // GENERIC REPOSITORY
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

            //SEED DATA
            services.AddScoped<DatabaseSeeder>();

            // REPOSITORIES
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


            // SERVICE MANAGERS
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


        }
    }
}
