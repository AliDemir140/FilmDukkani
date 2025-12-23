using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Extensions;
using MVC.Models;

namespace MVC.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "Cart";
        private readonly MovieServiceManager _movieService;
        private readonly DeliveryRequestServiceManager _deliveryRequestService;

        public CartController(
            MovieServiceManager movieService,
            DeliveryRequestServiceManager deliveryRequestService)
        {
            _movieService = movieService;
            _deliveryRequestService = deliveryRequestService;
        }

        // =========================
        // CART INDEX
        // =========================
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            return View(cart);
        }

        // =========================
        // ADD TO CART
        // =========================
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

        // =========================
        // REMOVE FROM CART
        // =========================
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

        // =========================
        // CHECKOUT (GET)
        // =========================
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction("Index");

            var lists = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Ana Liste" },
                new SelectListItem { Value = "2", Text = "Yedek Liste" }
            };

            var model = new CheckoutViewModel
            {
                CartItems = cart,
                MemberLists = lists,
                SelectedListId = int.Parse(lists.First().Value),
                DeliveryDate = DateTime.Today.AddDays(2)
            };

            return View(model);
        }

        // =========================
        // CHECKOUT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
                return View(model);

            int memberId = 1; // auth gelene kadar sabit

            var requestId = await _deliveryRequestService.CreateDeliveryRequestAsync(
                memberId,
                model.SelectedListId,
                model.DeliveryDate
            );

            if (requestId == 0)
            {
                ModelState.AddModelError("", "Teslimat isteği oluşturulamadı.");
                return View(model);
            }

            // Sepeti temizle
            HttpContext.Session.Remove(CartKey);

            return RedirectToAction("Success", new { id = requestId });
        }

        // =========================
        // SUCCESS PAGE
        // =========================
        public async Task<IActionResult> Success(int id)
        {
            var request = await _deliveryRequestService.GetRequestDetailAsync(id);
            if (request == null)
                return NotFound();

            var model = new CheckoutSuccessViewModel
            {
                DeliveryRequestId = request.Id,
                DeliveryDate = request.DeliveryDate,
                ListName = request.ListName
            };

            return View(model);
        }
    }
}
