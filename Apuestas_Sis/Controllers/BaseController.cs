using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Apuestas_Sis.Controllers;

public abstract class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Response.Headers["Cache-Control"] =
            "no-store, no-cache, must-revalidate, max-age=0";
        context.HttpContext.Response.Headers["Pragma"] = "no-cache";
        context.HttpContext.Response.Headers["Expires"] = "0";

        if (string.IsNullOrWhiteSpace(context.HttpContext.Session.GetString("UsuarioId")))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}