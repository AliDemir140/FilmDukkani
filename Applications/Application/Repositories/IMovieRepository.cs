using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        // İleride: En çok izlenen filmler, yıl filtreleme vs. ekleyebiliriz.
    }
}
