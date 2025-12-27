using Application.DTOs.MovieDTOs;
using System.Collections.Generic;

namespace MVC.Models
{
    public class HomeIndexViewModel
    {
        public List<MovieDto> EditorsChoice { get; set; } = new();
        public List<MovieDto> NewReleases { get; set; } = new();
    }
}
