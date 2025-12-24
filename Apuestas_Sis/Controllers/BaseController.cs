using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Apuestas_Sis.Controllers;

public abstract class BaseController : Controller
{
    protected bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("UsuarioId"));

    protected string[] Roles =>
        (HttpContext.Session.GetString("Roles") ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    protected bool HasRole(params string[] requiredRoles)
    {
        if (requiredRoles == null || requiredRoles.Length == 0) return true;
        return requiredRoles.Any(r => Roles.Contains(r, StringComparer.OrdinalIgnoreCase));
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        context.HttpContext.Response.Headers["Pragma"] = "no-cache";
        context.HttpContext.Response.Headers["Expires"] = "0";

        if (!IsAuthenticated)
        {
            context.Result = RedirectToAction("Login", "Account");
            return;
        }

        base.OnActionExecuting(context);
    }
}