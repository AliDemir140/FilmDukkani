using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Models;
using MVC.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieApiService _movieApiService;

        public HomeController(ILogger<HomeController> logger, MovieApiService movieApiService)
        {
            _logger = logger;
            _movieApiService = movieApiService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexViewModel
            {
                EditorsChoice = await _movieApiService.GetEditorsChoiceAsync(),
                NewReleases = await _movieApiService.GetNewReleasesAsync(),
                TopRented = await _movieApiService.GetTopRentedAsync(10)
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
