using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.DashBoard.Models;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireLogin]
    public class HomeController : DashBoardBaseController
    {
        private readonly MovieServiceManager _movieService;
        private readonly CategoryServiceManager _categoryService;
        private readonly MemberServiceManager _memberService;
        private readonly DeliveryRequestServiceManager _deliveryService;
        private readonly MovieCopyServiceManager _movieCopyService;

        public HomeController(
            MovieServiceManager movieService,
            CategoryServiceManager categoryService,
            MemberServiceManager memberService,
            DeliveryRequestServiceManager deliveryService,
            MovieCopyServiceManager movieCopyService)
        {
            _movieService = movieService;
            _categoryService = categoryService;
            _memberService = memberService;
            _deliveryService = deliveryService;
            _movieCopyService = movieCopyService;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetMoviesAsync();
            var categories = await _categoryService.GetCategoriesAsync();
            var members = await _memberService.GetMembersAsync();
            var requests = await _deliveryService.GetAllRequestsAsync();
            var copies = await _movieCopyService.GetMovieCopiesAsync();

            var pendingCount = requests.Count(x => x.Status == Domain.Enums.DeliveryStatus.Pending);
            var preparedCount = requests.Count(x => x.Status == Domain.Enums.DeliveryStatus.Prepared);
            var shippedCount = requests.Count(x => x.Status == Domain.Enums.DeliveryStatus.Shipped);
            var deliveredCount = requests.Count(x => x.Status == Domain.Enums.DeliveryStatus.Delivered);

            var damagedCopyCount = copies.Count(x => x.IsDamaged);

            var model = new DashboardViewModel
            {
                TotalMovies = movies.Count,
                TotalCategories = categories.Count,
                TotalMembers = members.Count,

                PendingDeliveries = pendingCount,
                PreparedDeliveries = preparedCount,
                ShippedDeliveries = shippedCount,
                DeliveredDeliveries = deliveredCount,

                DamagedCopies = damagedCopyCount,

                LastRequests = requests
                    .OrderByDescending(x => x.Id)
                    .Take(8)
                    .ToList()
            };

            return View(model);
        }
    }
}
