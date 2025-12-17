using Apuestas_Sis.Models;
using Apuestas_Sis.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Apuestas_Sis.Controllers
{
    public class UsuarioRolsController : Controller
    {
        private readonly ApuestasDataContext _context;

        public UsuarioRolsController(ApuestasDataContext context)
        {
            _context = context;
        }

        // GET: UsuarioRols
        public async Task<IActionResult> Index()
        {
            var apuestasDataContext = _context.UsuarioRoles.Include(u => u.Agencia).Include(u => u.Rol).Include(u => u.Usuario);
            return View(await apuestasDataContext.ToListAsync());
        }

        // GET: UsuarioRols/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var roles = await _context.UsuarioRoles
                .Include(x => x.Usuario)
                .Include(x => x.Rol)
                .Include(x => x.Agencia)
                .AsNoTracking()
                .Where(x => x.UsuarioId == id.Value)
                .OrderByDescending(x => x.Activo)
                .ThenByDescending(x => x.FechaAsignacion)
                .ToListAsync();

            if (roles.Count == 0) return NotFound();

            var usuario = roles.First().Usuario;

            var vm = new UsuarioRolesDetailsVM
            {
                UsuarioId = usuario.UsuarioId,
                UsuarioNombre = $"{usuario.Apellidos} {usuario.Nombres}",
                Roles = roles
            };

            return View(vm);
        }

        // GET: UsuarioRols/Create
        public IActionResult Create()
        {
            CargarCombos();
            return View(new UsuarioRol
            {
                Activo = true,
                EsGlobal = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioRol usuarioRol)
        {
            // ===== Reglas de negocio =====

            if (usuarioRol.EsGlobal)
            {
                // Rol global NO debe tener agencia
                usuarioRol.AgenciaId = null;
            }
            else
            {
                if (usuarioRol.AgenciaId == null)
                {
                    ModelState.AddModelError(nameof(UsuarioRol.AgenciaId),
                        "Debe seleccionar una agencia cuando el rol no es global.");
                }
            }

            // Auditoría controlada en servidor
            usuarioRol.FechaAsignacion = DateTime.Now;
            usuarioRol.FechaDesactivacion = usuarioRol.Activo ? null : DateTime.Now;

            // ===== Evitar duplicados (antes de guardar) =====
            if (ModelState.IsValid)
            {
                bool existe;

                if (usuarioRol.EsGlobal)
                {
                    existe = await _context.UsuarioRoles.AnyAsync(x =>
                        x.UsuarioId == usuarioRol.UsuarioId &&
                        x.RolId == usuarioRol.RolId &&
                        x.EsGlobal &&
                        x.AgenciaId == null
                    );
                }
                else
                {
                    existe = await _context.UsuarioRoles.AnyAsync(x =>
                        x.UsuarioId == usuarioRol.UsuarioId &&
                        x.RolId == usuarioRol.RolId &&
                        !x.EsGlobal &&
                        x.AgenciaId == usuarioRol.AgenciaId
                    );
                }

                if (existe)
                {
                    ModelState.AddModelError(string.Empty,
                        "Ya existe una asignación del mismo rol para este usuario.");
                }
            }

            if (!ModelState.IsValid)
            {
                CargarCombos(usuarioRol);
                return View(usuarioRol);
            }

            // PK (solo por seguridad)
            if (usuarioRol.UsuarioRolId == Guid.Empty)
                usuarioRol.UsuarioRolId = Guid.NewGuid();

            _context.UsuarioRoles.Add(usuarioRol);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: UsuarioRols/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var usuarioRol = await _context.UsuarioRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UsuarioRolId == id.Value);

            if (usuarioRol == null) return NotFound();

            ViewData["AgenciaId"] = new SelectList(
                await _context.Agencias.AsNoTracking().ToListAsync(),
                "AgenciaId", "Nombre", usuarioRol.AgenciaId);

            ViewData["RolId"] = new SelectList(
                await _context.Roles.AsNoTracking().ToListAsync(),
                "RolId", "Nombre", usuarioRol.RolId);

            ViewData["UsuarioId"] = new SelectList(
                await _context.Usuarios.AsNoTracking()
                    .Select(u => new { u.UsuarioId, Nombre = u.Apellidos + " " + u.Nombres })
                    .ToListAsync(),
                "UsuarioId", "Nombre", usuarioRol.UsuarioId);

            return View(usuarioRol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UsuarioRol usuarioRol)
        {
            if (id != usuarioRol.UsuarioRolId) return NotFound();

            var entity = await _context.UsuarioRoles
                .FirstOrDefaultAsync(x => x.UsuarioRolId == id);

            if (entity == null) return NotFound();

            // ===== Reglas de negocio =====
            if (usuarioRol.EsGlobal)
            {
                usuarioRol.AgenciaId = null; // global => sin agencia
            }
            else
            {
                if (usuarioRol.AgenciaId == null)
                {
                    ModelState.AddModelError(nameof(UsuarioRol.AgenciaId),
                        "Debe seleccionar una agencia cuando el rol no es global.");
                }
            }

            // ===== Evitar duplicados (excluye el actual) =====
            if (ModelState.IsValid)
            {
                bool existe;

                if (usuarioRol.EsGlobal)
                {
                    existe = await _context.UsuarioRoles.AnyAsync(x =>
                        x.UsuarioRolId != id &&
                        x.UsuarioId == usuarioRol.UsuarioId &&
                        x.RolId == usuarioRol.RolId &&
                        x.EsGlobal == true &&
                        x.AgenciaId == null
                    );
                }
                else
                {
                    existe = await _context.UsuarioRoles.AnyAsync(x =>
                        x.UsuarioRolId != id &&
                        x.UsuarioId == usuarioRol.UsuarioId &&
                        x.RolId == usuarioRol.RolId &&
                        x.EsGlobal == false &&
                        x.AgenciaId == usuarioRol.AgenciaId
                    );
                }

                if (existe)
                {
                    ModelState.AddModelError(string.Empty,
                        "Ya existe una asignación del mismo rol para este usuario.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Recargar combos dentro de la misma acción
                ViewData["AgenciaId"] = new SelectList(
                    await _context.Agencias.AsNoTracking().ToListAsync(),
                    "AgenciaId", "Nombre", usuarioRol.AgenciaId);

                ViewData["RolId"] = new SelectList(
                    await _context.Roles.AsNoTracking().ToListAsync(),
                    "RolId", "Nombre", usuarioRol.RolId);

                ViewData["UsuarioId"] = new SelectList(
                    await _context.Usuarios.AsNoTracking()
                        .Select(u => new { u.UsuarioId, Nombre = u.Apellidos + " " + u.Nombres })
                        .ToListAsync(),
                    "UsuarioId", "Nombre", usuarioRol.UsuarioId);

                return View(usuarioRol);
            }

            // ===== Actualización controlada (sin overposting) =====
            entity.UsuarioId = usuarioRol.UsuarioId;
            entity.RolId = usuarioRol.RolId;
            entity.EsGlobal = usuarioRol.EsGlobal;
            entity.AgenciaId = usuarioRol.AgenciaId;
            entity.Activo = usuarioRol.Activo;

            // Fechas automáticas
            entity.FechaDesactivacion = entity.Activo ? null : (entity.FechaDesactivacion ?? DateTime.Now);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioRolExists(entity.UsuarioRolId)) return NotFound();
                throw;
            }
        }

        // GET: UsuarioRols/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioRol = await _context.UsuarioRoles
                .Include(u => u.Agencia)
                .Include(u => u.Rol)
                .Include(u => u.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioRolId == id);
            if (usuarioRol == null)
            {
                return NotFound();
            }

            return View(usuarioRol);
        }

        // POST: UsuarioRols/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var usuarioRol = await _context.UsuarioRoles.FindAsync(id);
            if (usuarioRol != null)
            {
                _context.UsuarioRoles.Remove(usuarioRol);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioRolExists(Guid id)
        {
            return _context.UsuarioRoles.Any(e => e.UsuarioRolId == id);
        }

        private void CargarCombos(UsuarioRol? model = null)
        {
            ViewData["AgenciaId"] = new SelectList(
                _context.Agencias.AsNoTracking(),
                "AgenciaId",
                "Nombre",
                model?.AgenciaId
            );

            ViewData["RolId"] = new SelectList(
                _context.Roles.AsNoTracking(),
                "RolId",
                "Nombre",
                model?.RolId
            );

            ViewData["UsuarioId"] = new SelectList(
                _context.Usuarios.AsNoTracking()
                    .Select(u => new
                    {
                        u.UsuarioId,
                        Nombre = u.Apellidos + " " + u.Nombres
                    }),
                "UsuarioId",
                "Nombre",
                model?.UsuarioId
            );
        }
    }
}