using Apuestas_Sis.Filters;
using Apuestas_Sis.Models;
using Apuestas_Sis.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Apuestas_Sis.Controllers
{
    [RoleAuthorize("administrador")]
    public class UsuarioAgenciasController : Controller
    {
        private readonly ApuestasDataContext _context;

        public UsuarioAgenciasController(ApuestasDataContext context)
        {
            _context = context;
        }

        // GET: UsuarioAgencias
        public async Task<IActionResult> Index()
        {
            var data = await _context.UsuarioAgencias
                .Include(x => x.Usuario)
                .Include(x => x.Agencia)
                .AsNoTracking()
                .ToListAsync();

            return View(data);
        }

        // GET: UsuarioAgencias/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var lista = await _context.UsuarioAgencias
                .Include(x => x.Usuario)
                .Include(x => x.Agencia)
                .AsNoTracking()
                .Where(x => x.UsuarioId == id.Value)
                .OrderByDescending(x => x.Activo)
                .ThenByDescending(x => x.FechaAsignacion)
                .ToListAsync();

            if (lista.Count == 0) return NotFound();

            var usuario = lista.First().Usuario;

            var vm = new UsuarioAgenciasDetailsVM
            {
                UsuarioId = usuario.UsuarioId,
                UsuarioNombre = $"{usuario.Apellidos} {usuario.Nombres}",
                Activas = lista.Where(x => x.Activo).ToList(),
                Inactivas = lista.Where(x => !x.Activo).ToList()
            };

            return View(vm);
        }

        // GET: UsuarioAgencias/Create
        public IActionResult Create()
        {
            ViewData["AgenciaId"] = new SelectList(_context.Agencias, "AgenciaId", "Nombre");

            ViewData["UsuarioId"] = new SelectList(
                _context.Usuarios.Select(u => new
                {
                    u.UsuarioId,
                    Nombre = u.Apellidos + " " + u.Nombres
                }),
                "UsuarioId",
                "Nombre"
            );

            return View(new UsuarioAgencia { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioAgencia usuarioAgencia)
        {
            // Fechas automáticas (no vienen de la vista)
            usuarioAgencia.FechaAsignacion = DateTime.Now;
            usuarioAgencia.FechaDesactivacion = usuarioAgencia.Activo ? null : DateTime.Now;

            // Evitar duplicado por PK compuesta (UsuarioId + AgenciaId)
            if (ModelState.IsValid)
            {
                var existe = await _context.UsuarioAgencias.AnyAsync(x =>
                    x.UsuarioId == usuarioAgencia.UsuarioId &&
                    x.AgenciaId == usuarioAgencia.AgenciaId);

                if (existe)
                {
                    ModelState.AddModelError(string.Empty, "El usuario ya está asignado a esta agencia.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Recargar combos en la misma acción
                ViewData["AgenciaId"] = new SelectList(
                    await _context.Agencias.AsNoTracking().ToListAsync(),
                    "AgenciaId", "Nombre", usuarioAgencia.AgenciaId);

                ViewData["UsuarioId"] = new SelectList(
                    await _context.Usuarios.AsNoTracking()
                        .Select(u => new { u.UsuarioId, Nombre = u.Apellidos + " " + u.Nombres })
                        .ToListAsync(),
                    "UsuarioId", "Nombre", usuarioAgencia.UsuarioId);

                return View(usuarioAgencia);
            }

            _context.UsuarioAgencias.Add(usuarioAgencia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: UsuarioAgencias/Edit/5
        public async Task<IActionResult> Edit(Guid usuarioId, Guid agenciaId)
        {
            var usuarioAgencia = await _context.UsuarioAgencias
                .Include(x => x.Usuario)
                .Include(x => x.Agencia)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.AgenciaId == agenciaId);

            if (usuarioAgencia == null)
                return NotFound();

            return View(usuarioAgencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioAgencia usuarioAgencia)
        {
            // PK compuesta viene en hidden
            var entity = await _context.UsuarioAgencias
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioAgencia.UsuarioId &&
                                          x.AgenciaId == usuarioAgencia.AgenciaId);

            if (entity == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // Recargar navegación para que no falle la vista informativa
                var reload = await _context.UsuarioAgencias
                    .Include(x => x.Usuario)
                    .Include(x => x.Agencia)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UsuarioId == usuarioAgencia.UsuarioId &&
                                              x.AgenciaId == usuarioAgencia.AgenciaId);

                return View(reload ?? usuarioAgencia);
            }

            // Actualización controlada
            entity.Activo = usuarioAgencia.Activo;

            // Fechas automáticas
            if (entity.Activo)
                entity.FechaDesactivacion = null;
            else
                entity.FechaDesactivacion = entity.FechaDesactivacion ?? DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioAgenciaExists(usuarioAgencia.UsuarioId, usuarioAgencia.AgenciaId))
                    return NotFound();

                throw;
            }
        }

        // GET: UsuarioAgencias/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioAgencia = await _context.UsuarioAgencias
                .Include(u => u.Agencia)
                .Include(u => u.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuarioAgencia == null)
            {
                return NotFound();
            }

            return View(usuarioAgencia);
        }

        // POST: UsuarioAgencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var usuarioAgencia = await _context.UsuarioAgencias.FindAsync(id);
            if (usuarioAgencia != null)
            {
                _context.UsuarioAgencias.Remove(usuarioAgencia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioAgenciaExists(Guid usuarioId, Guid agenciaId)
        {
            return _context.UsuarioAgencias.Any(e => e.UsuarioId == usuarioId && e.AgenciaId == agenciaId);
        }
    }
}