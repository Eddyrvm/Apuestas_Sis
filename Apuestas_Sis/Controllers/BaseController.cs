using Microsoft.AspNetCore.Mvc;

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

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        // 1) Validar sesión
        if (!IsAuthenticated)
        {
            context.Result = RedirectToAction("Login", "Account");
            return;
        }

        base.OnActionExecuting(context);
    }
}