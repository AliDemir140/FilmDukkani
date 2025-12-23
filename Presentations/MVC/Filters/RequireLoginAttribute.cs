using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVC.Filters
{
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var token = httpContext.Session.GetString("JWT");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Auth",
                    null
                );
            }

            base.OnActionExecuting(context);
        }
    }
}
