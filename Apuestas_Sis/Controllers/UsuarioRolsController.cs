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
            // 1) Reglas Global/Agencia + FORZAR Administrador
            if (usuarioRol.EsGlobal)
            {
                usuarioRol.AgenciaId = null;

                var rolAdminId = await ObtenerRolAdministradorIdAsync();
                if (rolAdminId == Guid.Empty)
                {
                    ModelState.AddModelError(string.Empty,
                        "No existe el rol 'Administrador' en la base de datos. Cree el rol e intente nuevamente.");
                }
                else
                {
                    usuarioRol.RolId = rolAdminId; // FORZADO
                }
            }
            else
            {
                if (usuarioRol.AgenciaId == null)
                    ModelState.AddModelError(nameof(UsuarioRol.AgenciaId),
                        "Debe seleccionar una agencia cuando el rol no es global.");

                if (usuarioRol.RolId == Guid.Empty)
                    ModelState.AddModelError(nameof(UsuarioRol.RolId),
                        "El rol es obligatorio.");
            }

            // 2) Regla: si ya tiene Global activo, no permitir más asignaciones
            var yaTieneGlobalActivo = await _context.UsuarioRoles.AnyAsync(x =>
                x.UsuarioId == usuarioRol.UsuarioId &&
                x.Activo &&
                x.EsGlobal &&
                x.AgenciaId == null);

            if (yaTieneGlobalActivo)
            {
                ModelState.AddModelError(string.Empty,
                    "El usuario ya tiene un rol GLOBAL activo. No se permiten asignaciones adicionales.");
            }

            // 3) Auditoría
            usuarioRol.FechaAsignacion = DateTime.Now;
            usuarioRol.FechaDesactivacion = usuarioRol.Activo ? null : DateTime.Now;

            // 4) Duplicados
            if (ModelState.IsValid)
            {
                var existe = await _context.UsuarioRoles.AnyAsync(x =>
                    x.UsuarioId == usuarioRol.UsuarioId &&
                    x.RolId == usuarioRol.RolId &&
                    x.EsGlobal == usuarioRol.EsGlobal &&
                    x.AgenciaId == (usuarioRol.EsGlobal ? null : usuarioRol.AgenciaId));

                if (existe)
                    ModelState.AddModelError(string.Empty, "Ya existe una asignación del mismo rol para este usuario.");
            }

            if (!ModelState.IsValid)
            {
                CargarCombos(usuarioRol);
                return View(usuarioRol);
            }

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

            var entity = await _context.UsuarioRoles.FirstOrDefaultAsync(x => x.UsuarioRolId == id);
            if (entity == null) return NotFound();

            // 1) Reglas Global/Agencia + FORZAR Administrador
            if (usuarioRol.EsGlobal)
            {
                usuarioRol.AgenciaId = null;

                var rolAdminId = await ObtenerRolAdministradorIdAsync();
                if (rolAdminId == Guid.Empty)
                {
                    ModelState.AddModelError(string.Empty,
                        "No existe el rol 'Administrador' en la base de datos. Cree el rol e intente nuevamente.");
                }
                else
                {
                    usuarioRol.RolId = rolAdminId; // FORZADO
                }
            }
            else
            {
                if (usuarioRol.AgenciaId == null)
                    ModelState.AddModelError(nameof(UsuarioRol.AgenciaId),
                        "Debe seleccionar una agencia cuando el rol no es global.");

                if (usuarioRol.RolId == Guid.Empty)
                    ModelState.AddModelError(nameof(UsuarioRol.RolId),
                        "El rol es obligatorio.");
            }

            // 2) Regla: si hay Global activo, solo permitir editar/desactivar la asignación Global
            var usuarioTieneGlobalActivo = await _context.UsuarioRoles.AnyAsync(x =>
                x.UsuarioId == usuarioRol.UsuarioId &&
                x.Activo &&
                x.EsGlobal &&
                x.AgenciaId == null);

            var esteRegistroEsGlobal = entity.EsGlobal && entity.AgenciaId == null;

            if (usuarioTieneGlobalActivo && !esteRegistroEsGlobal)
            {
                ModelState.AddModelError(string.Empty,
                    "El usuario tiene un rol GLOBAL activo. Solo se permite editar/desactivar la asignación GLOBAL.");
            }

            // 3) Duplicados (excluye el actual)
            if (ModelState.IsValid)
            {
                var existe = await _context.UsuarioRoles.AnyAsync(x =>
                    x.UsuarioRolId != id &&
                    x.UsuarioId == usuarioRol.UsuarioId &&
                    x.RolId == usuarioRol.RolId &&
                    x.EsGlobal == usuarioRol.EsGlobal &&
                    x.AgenciaId == (usuarioRol.EsGlobal ? null : usuarioRol.AgenciaId));

                if (existe)
                    ModelState.AddModelError(string.Empty, "Ya existe una asignación del mismo rol para este usuario.");
            }

            if (!ModelState.IsValid)
            {
                // Recargar combos
                ViewData["AgenciaId"] = new SelectList(await _context.Agencias.AsNoTracking().ToListAsync(), "AgenciaId", "Nombre", usuarioRol.AgenciaId);
                ViewData["RolId"] = new SelectList(await _context.Roles.AsNoTracking().ToListAsync(), "RolId", "Nombre", usuarioRol.RolId);
                ViewData["UsuarioId"] = new SelectList(await _context.Usuarios.AsNoTracking()
                    .Select(u => new { u.UsuarioId, Nombre = u.Apellidos + " " + u.Nombres }).ToListAsync(),
                    "UsuarioId", "Nombre", usuarioRol.UsuarioId);

                return View(usuarioRol);
            }

            // 4) Actualización controlada
            entity.UsuarioId = usuarioRol.UsuarioId;
            entity.RolId = usuarioRol.RolId;          // (Admin si Global)
            entity.EsGlobal = usuarioRol.EsGlobal;
            entity.AgenciaId = usuarioRol.AgenciaId;
            entity.Activo = usuarioRol.Activo;

            entity.FechaDesactivacion = entity.Activo ? null : (entity.FechaDesactivacion ?? DateTime.Now);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

        private async Task<Guid> ObtenerRolAdministradorIdAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(r => r.Nombre.ToLower() == "administrador")
                .Select(r => r.RolId)
                .FirstOrDefaultAsync();
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