using Apuestas_Sis.Models;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DbContext (EF Core 8 + SQL Server)
builder.Services.AddDbContext<ApuestasDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===== Session (variables de sesión para login/autorización) =====
builder.Services.AddDistributedMemoryCache(); // En producción multi-servidor: Redis/SQL Server

builder.Services.AddSession(options =>
{
    // Caducidad por inactividad (idle timeout)
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Cookie de SessionId (NO es cookie de autenticación, es de sesión)
    options.Cookie.Name = ".ApuestasSis.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    // Recomendado para producción
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict;           // Reduce CSRF
});

// Política general de cookies (aplica también a la cookie de sesión)
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

var app = builder.Build();

// ===== Pipeline =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// APLICA CookiePolicyOptions (antes de Session)
app.UseCookiePolicy();

// Session antes de mapear rutas
app.UseSession();

// (Opcional, recomendable): Manejo de errores de estado sin lambdas
// app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();