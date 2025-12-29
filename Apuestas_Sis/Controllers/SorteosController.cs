using Apuestas_Sis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Apuestas_Sis.Controllers
{
    public class SorteosController : Controller
    {
        private readonly ApuestasDataContext _context;

        public SorteosController(ApuestasDataContext context)
        {
            _context = context;
        }

        // GET: Sorteos
        public async Task<IActionResult> Index()
        {
            var apuestasDataContext = _context.Sorteos.Include(s => s.CreadoPorUsuario).Include(s => s.ModalidadApuesta).Include(s => s.TipoJuego);
            return View(await apuestasDataContext.ToListAsync());
        }

        // GET: Sorteos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sorteo = await _context.Sorteos
                .Include(s => s.CreadoPorUsuario)
                .Include(s => s.ModalidadApuesta)
                .Include(s => s.TipoJuego)
                .FirstOrDefaultAsync(m => m.SorteoId == id);
            if (sorteo == null)
            {
                return NotFound();
            }

            return View(sorteo);
        }

        // GET: Sorteos/Create
        public async Task<IActionResult> Create()
        {
            // Si quieres forzar que exista sesión también en GET:
            var userIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrWhiteSpace(userIdStr))
                return RedirectToAction("Login", "Account");

            ViewData["TipoJuegoId"] = new SelectList(
                await _context.TipoJuego.AsNoTracking().ToListAsync(),
                "TipoJuegoId", "Nombre");

            ViewData["ModalidadApuestaId"] = new SelectList(
                await _context.ModalidadApuestas.AsNoTracking()
                    .Select(m => new
                    {
                        m.ModalidadApuestaId,
                        Texto = !string.IsNullOrWhiteSpace(m.Descripcion)
                            ? m.Descripcion
                            : ("Modalidad " + m.Cifras + " cifras")
                    })
                    .ToListAsync(),
                "ModalidadApuestaId", "Texto");

            return View(new Sorteo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sorteo sorteo)
        {
            // ===== 1) Usuario logeado desde Session =====
            var userIdStr = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrWhiteSpace(userIdStr))
            {
                // No hay sesión -> obligar login
                return RedirectToAction("Login", "Account");
            }

            if (!Guid.TryParse(userIdStr, out var userId))
            {
                ModelState.AddModelError(string.Empty,
                    "No se pudo identificar el usuario logeado. Vuelva a iniciar sesión.");
            }
            else
            {
                // Asignar auditoría SOLO si el userId es válido
                sorteo.CreadoPorUsuarioId = userId;
            }

            // ===== 2) Auditoría controlada en servidor =====
            if (sorteo.SorteoId == Guid.Empty)
                sorteo.SorteoId = Guid.NewGuid();

            sorteo.FechaCreacion = DateTime.Now;

            // Estado inicial (si no lo envías desde la vista)
            if (string.IsNullOrWhiteSpace(sorteo.Estado))
                sorteo.Estado = "ABIERTO";

            // Validación mínima (opcional recomendado)
            if (sorteo.FechaSorteo < DateTime.Now.AddMinutes(-1))
            {
                ModelState.AddModelError(nameof(Sorteo.FechaSorteo),
                    "La fecha del sorteo no puede ser anterior a la fecha actual.");
            }

            // ===== 3) Guardar / recargar combos =====
            if (!ModelState.IsValid)
            {
                ViewData["TipoJuegoId"] = new SelectList(
                    await _context.TipoJuego.AsNoTracking().ToListAsync(),
                    "TipoJuegoId", "Nombre", sorteo.TipoJuegoId);

                ViewData["ModalidadApuestaId"] = new SelectList(
                    await _context.ModalidadApuestas.AsNoTracking()
                        .Select(m => new
                        {
                            m.ModalidadApuestaId,
                            Texto = !string.IsNullOrWhiteSpace(m.Descripcion)
                                ? m.Descripcion
                                : ("Modalidad " + m.Cifras + " cifras")
                        })
                        .ToListAsync(),
                    "ModalidadApuestaId", "Texto", sorteo.ModalidadApuestaId);

                return View(sorteo);
            }

            _context.Sorteos.Add(sorteo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Sorteos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sorteo = await _context.Sorteos.FindAsync(id);
            if (sorteo == null)
            {
                return NotFound();
            }
            ViewData["CreadoPorUsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellidos", sorteo.CreadoPorUsuarioId);
            ViewData["ModalidadApuestaId"] = new SelectList(_context.ModalidadApuestas, "ModalidadApuestaId", "ModalidadApuestaId", sorteo.ModalidadApuestaId);
            ViewData["TipoJuegoId"] = new SelectList(_context.TipoJuego, "TipoJuegoId", "Nombre", sorteo.TipoJuegoId);
            return View(sorteo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Sorteo sorteo)
        {
            if (id != sorteo.SorteoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sorteo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SorteoExists(sorteo.SorteoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreadoPorUsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellidos", sorteo.CreadoPorUsuarioId);
            ViewData["ModalidadApuestaId"] = new SelectList(_context.ModalidadApuestas, "ModalidadApuestaId", "ModalidadApuestaId", sorteo.ModalidadApuestaId);
            ViewData["TipoJuegoId"] = new SelectList(_context.TipoJuego, "TipoJuegoId", "Nombre", sorteo.TipoJuegoId);
            return View(sorteo);
        }

        // GET: Sorteos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sorteo = await _context.Sorteos
                .Include(s => s.CreadoPorUsuario)
                .Include(s => s.ModalidadApuesta)
                .Include(s => s.TipoJuego)
                .FirstOrDefaultAsync(m => m.SorteoId == id);
            if (sorteo == null)
            {
                return NotFound();
            }

            return View(sorteo);
        }

        // POST: Sorteos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var sorteo = await _context.Sorteos.FindAsync(id);
            if (sorteo != null)
            {
                _context.Sorteos.Remove(sorteo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SorteoExists(Guid id)
        {
            return _context.Sorteos.Any(e => e.SorteoId == id);
        }
    }
}