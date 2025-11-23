using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts
{
    public class FilmDukkaniDbContext : DbContext
    {
        public FilmDukkaniDbContext (DbContextOptions<FilmDukkaniDbContext> options) : base(options) 
        { 
                    
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        
    }
}
