using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVC.Constants;
using System;

namespace MVC.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public RequireRoleAttribute(params string[] roles)
        {
            _roles = roles ?? Array.Empty<string>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 1) AllowAnonymous varsa dokunma (bazı sayfalar için kaçış)
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                base.OnActionExecuting(context);
                return;
            }

            // 2) Redirect loop kırıcı: Dashboard/Home/AccessDenied'e dokunma
            var area = context.RouteData.Values["area"]?.ToString() ?? "";
            var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
            var action = context.RouteData.Values["action"]?.ToString() ?? "";

            if (string.Equals(area, "DashBoard", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(controller, "Home", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(action, "AccessDenied", StringComparison.OrdinalIgnoreCase))
            {
                base.OnActionExecuting(context);
                return;
            }

            var session = context.HttpContext.Session;

            var token = session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", new { area = "" });
                return;
            }

            var role = session.GetString(SessionKeys.Role);
            role = string.IsNullOrWhiteSpace(role) ? "User" : role;

            if (_roles.Length > 0 && !_roles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                if (string.Equals(area, "DashBoard", StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Home", new { area = "DashBoard" });
                    return;
                }

                context.Result = new RedirectToActionResult("Index", "Home", new { area = "" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
