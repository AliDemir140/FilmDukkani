using Application.DTOs.MovieDTOs;

namespace MVC.Models
{
    public class HomeIndexViewModel
    {
        public List<MovieDto> EditorsChoice { get; set; } = new();
        public List<MovieDto> NewReleases { get; set; } = new();
        public List<MovieDto> TopRented { get; set; } = new();

        public List<MovieDto> AwardWinners { get; set; } = new();
    }
}
