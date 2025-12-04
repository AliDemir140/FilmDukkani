using Application.Repositories;
using Application.ServiceManager;
using Infrastructure.Persistence;
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

            // CONCRETE REPOSITORIES
            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IMembershipPlanRepository, MembershipPlanRepository>();
            services.AddScoped<IMemberMovieListRepository, MemberMovieListRepository>();
            services.AddScoped<IMemberMovieListItemRepository, MemberMovieListItemRepository>();
            services.AddScoped<IDeliveryRequestRepository, DeliveryRequestRepository>();
            services.AddScoped<IDeliveryRequestItemRepository, DeliveryRequestItemRepository>();


            // SERVICE MANAGERS
            services.AddScoped<CategoryServiceManager>();
            services.AddScoped<MovieServiceManager>();
            services.AddScoped<MemberServiceManager>();
            services.AddScoped<MembershipPlanServiceManager>();
            services.AddScoped<MemberMovieListServiceManager>();
            services.AddScoped<DeliveryRequestServiceManager>();


        }
    }
}
