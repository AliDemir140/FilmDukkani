using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class FilmDukkaniDbContext : DbContext
    {
        public FilmDukkaniDbContext (DbContextOptions<FilmDukkaniDbContext> options) : base(options) 
        { 
                    
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data
            SeedData.DatabaseSeeder.SeedData(modelBuilder);
        }

        //DBSET
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<MemberMovieList> MemberMovieLists { get; set; }
        public DbSet<MemberMovieListItem> MemberMovieListItems { get; set; }


    }
}
