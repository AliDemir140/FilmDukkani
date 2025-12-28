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

        private string? ApiBaseUrl => _configuration["ApiSettings:BaseUrl"];

        private class CheckMinimumResponse
        {
            public bool hasMinimum { get; set; }
            public int minimumRequired { get; set; }
            public int currentCount { get; set; }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(SessionKeys.Cart);
            HttpContext.Session.SetObject(SessionKeys.Cart, new List<CartItem>());

            TempData["Success"] = "Sepet temizlendi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetMemberListsSelectAsync(int memberId)
        {
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return new List<SelectListItem>();

            try
            {
                var client = _httpClientFactory.CreateClient();

                var lists = await client.GetFromJsonAsync<List<MemberMovieListDto>>(
                    $"{ApiBaseUrl}/api/MemberMovieList/lists-by-member?memberId={memberId}"
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
                return new List<SelectListItem>();
            }
        }

        private async Task<(bool ok, int currentCount, int minimumRequired)> CheckMinimumAsync(int listId)
        {
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return (false, 0, 5);

            try
            {
                var client = _httpClientFactory.CreateClient();

                var res = await client.GetFromJsonAsync<CheckMinimumResponse>(
                    $"{ApiBaseUrl}/api/MemberMovieList/check-minimum?listId={listId}"
                );

                if (res == null)
                    return (false, 0, 5);

                return (res.hasMinimum, res.currentCount, res.minimumRequired);
            }
            catch
            {
                return (false, 0, 5);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckMinimum(int listId)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return Unauthorized();

            if (listId <= 0)
                return BadRequest("listId zorunludur.");

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return BadRequest("ApiSettings:BaseUrl bulunamadı.");

            try
            {
                var client = _httpClientFactory.CreateClient();

                var res = await client.GetAsync($"{ApiBaseUrl}/api/MemberMovieList/check-minimum?listId={listId}");
                if (!res.IsSuccessStatusCode)
                    return StatusCode((int)res.StatusCode);

                var data = await res.Content.ReadFromJsonAsync<CheckMinimumResponse>();
                if (data == null)
                    return StatusCode(500);

                return Ok(data);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        private async Task AddCartMoviesToListAsync(int listId, List<CartItem> cart)
        {
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return;

            try
            {
                var client = _httpClientFactory.CreateClient();

                var existingItems = await client.GetFromJsonAsync<List<MemberMovieListItemDto>>(
                    $"{ApiBaseUrl}/api/MemberMovieList/list-items?listId={listId}"
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

                    await client.PostAsJsonAsync($"{ApiBaseUrl}/api/MemberMovieList/add-item", dto);
                }
            }
            catch
            {
            }
        }

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
                return RedirectToAction("Create", "MyLists", new { returnUrl = "/Cart/Checkout" });
            }

            int selectedListId = int.Parse(memberLists.First().Value);

            var min = await CheckMinimumAsync(selectedListId);
            ViewBag.HasMinimum = min.ok;
            ViewBag.MinRequired = min.minimumRequired;
            ViewBag.CurrentCount = min.currentCount;

            var model = new CheckoutViewModel
            {
                CartItems = cart,
                MemberLists = memberLists,
                SelectedListId = selectedListId,
                DeliveryDate = DateTime.Today.AddDays(2)
            };

            return View(model);
        }

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

            if (!ModelState.IsValid)
            {
                model.CartItems = cart;
                model.MemberLists = await GetMemberListsSelectAsync(memberId.Value);

                if (model.MemberLists == null || !model.MemberLists.Any())
                {
                    TempData["Error"] = "Checkout yapabilmek için önce bir liste oluşturmalısın.";
                    return RedirectToAction("Create", "MyLists", new { returnUrl = "/Cart/Checkout" });
                }

                if (model.SelectedListId <= 0)
                    model.SelectedListId = int.Parse(model.MemberLists.First().Value);

                var minAgain = await CheckMinimumAsync(model.SelectedListId);
                ViewBag.HasMinimum = minAgain.ok;
                ViewBag.MinRequired = minAgain.minimumRequired;
                ViewBag.CurrentCount = minAgain.currentCount;

                return View(model);
            }

            await AddCartMoviesToListAsync(model.SelectedListId, cart);

            var min = await CheckMinimumAsync(model.SelectedListId);
            ViewBag.HasMinimum = min.ok;
            ViewBag.MinRequired = min.minimumRequired;
            ViewBag.CurrentCount = min.currentCount;

            if (!min.ok)
            {
                model.CartItems = cart;
                model.MemberLists = await GetMemberListsSelectAsync(memberId.Value);

                ModelState.AddModelError("", $"Teslimat için seçili listede en az {min.minimumRequired} film olmalı. Şu an: {min.currentCount}");
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

                ModelState.AddModelError("",
                    "Teslimat isteği oluşturulamadı. (Teslimat tarihi en az 2 gün sonrası olmalı ve Pazar olamaz.)");

                return View(model);
            }

            if (requestId == -1)
            {
                model.CartItems = cart;
                model.MemberLists = await GetMemberListsSelectAsync(memberId.Value);

                ModelState.AddModelError("",
                    "Bu liste için zaten aktif bir sipariş var. (Bekliyor/Hazırlanıyor/Kuryede/Teslim Edildi). Lütfen farklı liste seçin.");

                return View(model);
            }

            if (requestId == -2)
            {
                model.CartItems = cart;
                model.MemberLists = await GetMemberListsSelectAsync(memberId.Value);

                ModelState.AddModelError("",
                    "Yeni teslimat talebi oluşturamazsın. Üst üste 2 teslimatta iade yapılmadığı için hesabın kısıtlandı. Lütfen önce iade işlemlerini tamamla veya admin ile iletişime geç.");

                return View(model);
            }

            if (requestId < 0)
            {
                model.CartItems = cart;
                model.MemberLists = await GetMemberListsSelectAsync(memberId.Value);

                ModelState.AddModelError("", "Teslimat isteği oluşturulamadı. Lütfen daha sonra tekrar dene.");
                return View(model);
            }

            HttpContext.Session.Remove(SessionKeys.Cart);
            HttpContext.Session.SetObject(SessionKeys.Cart, new List<CartItem>());

            return RedirectToAction(nameof(Success), new { id = requestId });
        }

        public async Task<IActionResult> Success(int id)
        {
            if (id <= 0)
                return BadRequest();

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
