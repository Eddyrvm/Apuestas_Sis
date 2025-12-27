using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Apuestas_Sis.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _rolesPermitidos;

    public RoleAuthorizeAttribute(params string[] rolesPermitidos)
    {
        _rolesPermitidos = rolesPermitidos ?? Array.Empty<string>();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Si no hay sesión -> Login (por seguridad)
        var usuarioId = context.HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        // Roles del usuario en sesión
        var rolesCsv = context.HttpContext.Session.GetString("Roles") ?? "";
        var rolesSesion = rolesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // Si no se especificaron roles, no restringe (opcional; puedes cambiarlo)
        if (_rolesPermitidos.Length == 0)
            return;

        var ok = _rolesPermitidos.Any(r => rolesSesion.Contains(r, StringComparer.OrdinalIgnoreCase));

        if (!ok)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
        }
    }
}