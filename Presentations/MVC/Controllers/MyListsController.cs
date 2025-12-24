using System.Net.Http.Json;
using Application.DTOs.MemberMovieListDTOs;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;
using MVC.Filters;

namespace MVC.Controllers
{
    [RequireLogin]
    public class MyListsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MyListsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: /MyLists
        public async Task<IActionResult> Index()
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];

            var lists = await client.GetFromJsonAsync<List<MemberMovieListDto>>(
                $"{apiBaseUrl}/api/MemberMovieList/lists-by-member?memberId={memberId.Value}"
            );

            lists ??= new List<MemberMovieListDto>();
            return View(lists);
        }

        // GET: /MyLists/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateMemberMovieListDto());
        }

        // POST: /MyLists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberMovieListDto dto)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            dto.MemberId = memberId.Value;

            if (!ModelState.IsValid)
                return View(dto);

            var client = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];

            var response = await client.PostAsJsonAsync(
                $"{apiBaseUrl}/api/MemberMovieList/create-list",
                dto
            );

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Liste oluşturulamadı.");
                return View(dto);
            }

            return RedirectToAction("Checkout", "Cart");

        }
    }
}
