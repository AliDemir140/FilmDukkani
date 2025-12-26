using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence
{
    public class FilmDukkaniDbContext
        : IdentityDbContext<IdentityUser>
    {
        public FilmDukkaniDbContext(DbContextOptions<FilmDukkaniDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilmDukkaniDbContext).Assembly);
        }

        // DBSET
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<MemberMovieList> MemberMovieLists { get; set; }
        public DbSet<MemberMovieListItem> MemberMovieListItems { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<MovieDirector> MovieDirectors { get; set; }
        public DbSet<MovieAward> MovieAwards { get; set; }
        public DbSet<DeliveryRequest> DeliveryRequests { get; set; }
        public DbSet<DeliveryRequestItem> DeliveryRequestItems { get; set; }
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<MovieCopy> MovieCopies { get; set; }
        public DbSet<DamagedMovie> DamagedMovies { get; set; }
        public DbSet<MovieCategory> MovieCategories { get; set; }

    }
}
