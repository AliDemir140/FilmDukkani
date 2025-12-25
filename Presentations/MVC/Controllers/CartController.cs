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
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionKeys.Cart)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.MovieId == id);
            if (item != null)
                cart.Remove(item);

            HttpContext.Session.SetObject(SessionKeys.Cart, cart);
            return RedirectToAction(nameof(Index));
        }

        // Üyenin listelerini API'den çekip SelectListItem'a çevirir
        private async Task<List<SelectListItem>> GetMemberListsSelectAsync(int memberId)
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
                return new List<SelectListItem>();

            try
            {
                var client = _httpClientFactory.CreateClient();

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
            catch
            {
                // API kapalı / hata -> boş dön, view patlamasın
                return new List<SelectListItem>();
            }
        }

        // ✅ Sepetteki filmleri seçilen listeye otomatik ekle
        // Not: Aynı film listede varsa tekrar eklemez (API zaten false döndürüyor)
        private async Task AddCartMoviesToListAsync(int listId, List<CartItem> cart)
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
                return;

            try
            {
                var client = _httpClientFactory.CreateClient();

                // Listede mevcut item'ları çek (priority devam etsin diye)
                var existingItems = await client.GetFromJsonAsync<List<MemberMovieListItemDto>>(
                    $"{apiBaseUrl}/api/MemberMovieList/list-items?listId={listId}"
                ) ?? new List<MemberMovieListItemDto>();

                var existingMovieIds = existingItems.Select(x => x.MovieId).ToHashSet();
                int nextPriority = existingItems.Any() ? existingItems.Max(x => x.Priority) + 1 : 1;

                foreach (var c in cart)
                {
                    if (existingMovieIds.Contains(c.MovieId))
                        continue;

                    var dto = new CreateMemberMovieListItemDto
                    {
                        MemberMovieListId = listId,
                        MovieId = c.MovieId,
                        Priority = nextPriority++
                    };

                    // Başarısız olsa bile checkout'u çökertmeyelim
                    await client.PostAsJsonAsync($"{apiBaseUrl}/api/MemberMovieList/add-item", dto);
                }
            }
            catch
            {
                // sessiz geç: checkout sayfası patlamasın
            }
        }

        // GET: /Cart/Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionKeys.Cart)
                       ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction(nameof(Index));

            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var memberLists = await GetMemberListsSelectAsync(memberId.Value);

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

        // POST: /Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionKeys.Cart)
                       ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction(nameof(Index));

            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            // ModelState invalid olursa listeyi geri doldur (API'den)
            if (!ModelState.IsValid)
            {
                model.CartItems = cart;
                model.MemberLists = await GetMemberListsSelectAsync(memberId.Value);

                if (model.MemberLists == null || !model.MemberLists.Any())
                {
                    TempData["Error"] = "Checkout yapabilmek için önce bir liste oluşturmalısın.";
                    return RedirectToAction("Create", "MyLists");
                }

                if (model.SelectedListId <= 0)
                    model.SelectedListId = int.Parse(model.MemberLists.First().Value);

                return View(model);
            }

            // ✅ KRİTİK FIX: Sepetteki filmleri seçilen listeye yaz
            await AddCartMoviesToListAsync(model.SelectedListId, cart);

            var requestId = await _deliveryRequestService.CreateDeliveryRequestAsync(
                memberId.Value,
                model.SelectedListId,
                model.DeliveryDate
            );

            if (requestId == 0)
            {
                model.CartItems = cart;
                model.MemberLists = await GetMemberListsSelectAsync(memberId.Value);

                ModelState.AddModelError("",
                    "Teslimat isteği oluşturulamadı. (Teslimat tarihi en az 2 gün sonrası olmalı ve Pazar olamaz.)");

                return View(model);
            }

            HttpContext.Session.Remove(SessionKeys.Cart);
            return RedirectToAction(nameof(Success), new { id = requestId });
        }

        // GET: /Cart/Success/5
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
