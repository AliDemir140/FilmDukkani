using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVC.Constants;

namespace MVC.Filters
{
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            var token = session.GetString(SessionKeys.JwtToken);
            var role = session.GetString(SessionKeys.Role);

            // Giriş yoksa
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Auth",
                    null
                );
                return;
            }

            // Admin değilse
            if (role != "Admin")
            {
                context.Result = new RedirectToActionResult(
                    "Index",
                    "Home",
                    null
                );
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
