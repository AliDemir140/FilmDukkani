using Application.DTOs.MemberMovieListDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Constants;
using MVC.Extensions;
using MVC.Filters;
using MVC.Models;
using System.Net.Http.Json;

namespace MVC.Controllers
{
    [RequireLogin]
    public class CartController : Controller
    {
        private readonly MovieServiceManager _movieService;
        private readonly DeliveryRequestServiceManager _deliveryRequestService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CartController(
            MovieServiceManager movieService,
            DeliveryRequestServiceManager deliveryRequestService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _movieService = movieService;
            _deliveryRequestService = deliveryRequestService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionKeys.Cart)
                       ?? new List<CartItem>();

            return View(cart);
        }

        public async Task<IActionResult> Add(int id)
        {
            var movie = (await _movieService.GetMoviesAsync())
                        .FirstOrDefault(x => x.Id == id);

            if (movie == null)
                return NotFound();

            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionKeys.Cart)
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

            HttpContext.Session.SetObject(SessionKeys.Cart, cart);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionKeys.Cart)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.MovieId == id);
            if (item != null)
                cart.Remove(item);

            HttpContext.Session.SetObject(SessionKeys.Cart, cart);
            return RedirectToAction("Index");
        }

        // Üyenin listelerini API'den çekip SelectListItem'a çevirir
        private async Task<List<SelectListItem>> GetMemberListsSelectAsync(int memberId)
        {
            var client = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];

            var lists = await client.GetFromJsonAsync<List<MemberMovieListDto>>(
                $"{apiBaseUrl}/api/MemberMovieList/lists-by-member?memberId={memberId}"
            );

            lists ??= new List<MemberMovieListDto>();

            return lists.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();
        }

        // LOGIN ZORUNLU
        [RequireLogin]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionKeys.Cart)
                       ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction("Index");

            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var memberLists = await GetMemberListsSelectAsync(memberId.Value);

            // Eğer hiç liste yoksa önce liste oluşturmaya yönlendir
            if (!memberLists.Any())
            {
                TempData["Error"] = "Checkout yapabilmek için önce bir liste oluşturmalısın.";
                return RedirectToAction("Create", "MyLists");
            }

            var model = new CheckoutViewModel
            {
                CartItems = cart,
                MemberLists = memberLists,
                SelectedListId = int.Parse(memberLists.First().Value),
                DeliveryDate = DateTime.Today.AddDays(2)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireLogin]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionKeys.Cart)
                       ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction("Index");

            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            // ModelState invalid olursa listeyi geri doldur (DB'den)
            if (!ModelState.IsValid)
            {
                model.CartItems = cart;
                model.MemberLists = await GetMemberListsSelectAsync(memberId.Value);

                if (model.MemberLists == null || !model.MemberLists.Any())
                {
                    TempData["Error"] = "Checkout yapabilmek için önce bir liste oluşturmalısın.";
                    return RedirectToAction("Create", "MyLists");
                }

                return View(model);
            }

            var requestId = await _deliveryRequestService.CreateDeliveryRequestAsync(
                memberId.Value,
                model.SelectedListId,
                model.DeliveryDate
            );

            if (requestId == 0)
            {
                model.CartItems = cart;
                model.MemberLists = await GetMemberListsSelectAsync(memberId.Value);

                ModelState.AddModelError("", "Teslimat isteği oluşturulamadı.");
                return View(model);
            }

            HttpContext.Session.Remove(SessionKeys.Cart);
            return RedirectToAction("Success", new { id = requestId });
        }

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
