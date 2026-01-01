using Application.Constants;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [ServiceFilter(typeof(RequireLoginAttribute))]
    [RequireRole(RoleNames.Admin, RoleNames.Accounting, RoleNames.Warehouse, RoleNames.Purchasing)]
    public abstract class DashBoardBaseController : Controller
    {
    }
}
