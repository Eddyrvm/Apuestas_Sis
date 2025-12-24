using Apuestas_Sis.Models;
using Apuestas_Sis.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apuestas_Sis.Controllers;

public class AccountController : Controller
{
    private readonly ApuestasDataContext _db;

    public AccountController(ApuestasDataContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        // Si ya está logueado, manda a Home
        if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("UsuarioId")))
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        // 1) Buscar usuario activo
        var usuario = await _db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UsuarioLogin == model.UsuarioLogin && u.Activo);

        if (usuario == null)
        {
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View(model);
        }

        // 2) Verificar password (BCrypt)
        var ok = BCrypt.Net.BCrypt.Verify(model.Password, usuario.PasswordHash);
        if (!ok)
        {
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View(model);
        }

        // 3) Cargar roles (N roles) activos del usuario
        var roles = await _db.UsuarioRoles
            .AsNoTracking()
            .Where(ur => ur.UsuarioId == usuario.UsuarioId && ur.Activo)
            .Include(ur => ur.Rol)
            .Where(ur => ur.Rol.Activo)
            .Select(ur => ur.Rol.Nombre)
            .Distinct()
            .ToListAsync();

        if (roles.Count == 0)
        {
            ModelState.AddModelError("", "El usuario no tiene roles asignados.");
            return View(model);
        }

        // 4) Cargar agencias activas asignadas al usuario
        var agencias = await _db.UsuarioAgencias
            .AsNoTracking()
            .Where(ua => ua.UsuarioId == usuario.UsuarioId && ua.Activo)
            .Include(ua => ua.Agencia)
            .Where(ua => ua.Agencia.Activo)
            .Select(ua => new { ua.AgenciaId, ua.Agencia.Nombre })
            .ToListAsync();

        if (agencias.Count == 0)
        {
            ModelState.AddModelError("", "El usuario no tiene agencias asignadas.");
            return View(model);
        }

        // 5) Guardar sesión (simple, como tu ejemplo)
        HttpContext.Session.SetString("UsuarioId", usuario.UsuarioId.ToString());
        HttpContext.Session.SetString("UsuarioLogin", usuario.UsuarioLogin);
        HttpContext.Session.SetString("NombreCompleto", $"{usuario.Nombres} {usuario.Apellidos}");

        // Roles como CSV para validar fácil en acciones/vistas
        HttpContext.Session.SetString("Roles", string.Join(",", roles));

        // Agencia: por simplicidad guardamos la primera.
        // Si luego quieres selección de agencia, se añade una vista/acción.
        HttpContext.Session.SetString("AgenciaId", agencias[0].AgenciaId.ToString());
        HttpContext.Session.SetString("AgenciaNombre", agencias[0].Nombre);

        // 6) Redirect como MVC clásico, cuidando returnUrl
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}