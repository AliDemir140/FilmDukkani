using Microsoft.AspNetCore.Mvc;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [ServiceFilter(typeof(RequireLoginAttribute))]
    public abstract class DashBoardBaseController : Controller
    {
    }
}
