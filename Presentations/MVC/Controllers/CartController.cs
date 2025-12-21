using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using MVC.Extensions;
using MVC.Models;

namespace MVC.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "Cart";
        private readonly MovieServiceManager _movieService;

        public CartController(MovieServiceManager movieService)
        {
            _movieService = movieService;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            return View(cart);
        }

        public async Task<IActionResult> Add(int id)
        {
            var movie = (await _movieService.GetMoviesAsync())
                        .FirstOrDefault(x => x.Id == id);

            if (movie == null)
                return NotFound();

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            if (!cart.Any(x => x.MovieId == id))
            {
                cart.Add(new CartItem
                {
                    MovieId = movie.Id,
                    MovieTitle = movie.Title,
                    CoverImageUrl = movie.CoverImageUrl,
                    ReleaseYear = movie.ReleaseYear
                });
            }

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.MovieId == id);
            if (item != null)
                cart.Remove(item);

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }
    }
}
