using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
