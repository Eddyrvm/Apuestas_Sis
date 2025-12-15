using Apuestas_Sis.Models;
using Apuestas_Sis.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apuestas_Sis.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApuestasDataContext _context;

        public UsuariosController(ApuestasDataContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View(new UsuarioCreateVm { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var usuario = new Usuario
            {
                UsuarioId = Guid.NewGuid(),
                UsuarioLogin = vm.UsuarioLogin.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.Password),
                Nombres = vm.Nombres.Trim(),
                Apellidos = vm.Apellidos.Trim(),
                Cedula = vm.Cedula?.Trim() ?? "",
                Telefono = vm.Telefono?.Trim() ?? "",
                Activo = vm.Activo,
                FechaRegistro = DateTime.Now
            };

            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            var vm = new UsuarioEditVm
            {
                UsuarioId = usuario.UsuarioId,
                UsuarioLogin = usuario.UsuarioLogin,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Cedula = usuario.Cedula,
                Telefono = usuario.Telefono,
                Activo = usuario.Activo,
                FechaRegistro = usuario.FechaRegistro
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UsuarioEditVm vm)
        {
            if (id != vm.UsuarioId) return NotFound();

            if (!ModelState.IsValid)
                return View(vm);

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.UsuarioLogin = vm.UsuarioLogin.Trim();
            usuario.Nombres = vm.Nombres.Trim();
            usuario.Apellidos = vm.Apellidos.Trim();
            usuario.Cedula = vm.Cedula?.Trim() ?? "";
            usuario.Telefono = vm.Telefono?.Trim() ?? "";
            usuario.Activo = vm.Activo;
            // FechaRegistro no se modifica

            if (!string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.NewPassword);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(Guid id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}