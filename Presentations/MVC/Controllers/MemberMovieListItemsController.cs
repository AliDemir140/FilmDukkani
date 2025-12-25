using Application.DTOs.MemberMovieListDTOs;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;
using MVC.Filters;
using System.Net.Http.Json;

namespace MVC.Controllers
{
    [RequireLogin]
    public class MemberMovieListItemsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MemberMovieListItemsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // POST: /MemberMovieListItems/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int listId, int movieId, string? returnUrl = null)
        {
            if (listId <= 0 || movieId <= 0)
            {
                TempData["Error"] = "Liste veya film bilgisi hatalı.";
                return Redirect(returnUrl ?? Url.Action("Index", "Product")!);
            }

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            var client = _httpClientFactory.CreateClient();

            // 1) Öncelik hesapla: listenin en sonuna ekle
            int nextPriority = 1;

            try
            {
                var items = await client.GetFromJsonAsync<List<MemberMovieListItemDto>>(
                    $"{apiBaseUrl}/api/MemberMovieList/list-items?listId={listId}"
                );

                if (items != null && items.Any())
                    nextPriority = items.Max(x => x.Priority) + 1;
            }
            catch
            {
                // ignore -> nextPriority 1 kalsın
            }

            // 2) Listeye ekle
            var dto = new CreateMemberMovieListItemDto
            {
                MemberMovieListId = listId,
                MovieId = movieId,
                Priority = nextPriority
            };

            var res = await client.PostAsJsonAsync($"{apiBaseUrl}/api/MemberMovieList/add-item", dto);

            if (res.IsSuccessStatusCode)
            {
                TempData["Success"] = "Film listeye eklendi ✅";
            }
            else
            {
                // API "Bu film zaten listede mevcut." döndürüyor
                var msg = await res.Content.ReadAsStringAsync();
                TempData["Error"] = string.IsNullOrWhiteSpace(msg)
                    ? "Film listeye eklenemedi."
                    : msg.Replace("\"", "");
            }

            return Redirect(returnUrl ?? Url.Action("Index", "Product")!);
        }
    }
}
