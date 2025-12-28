using Apuestas_Sis.Filters;
using Apuestas_Sis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apuestas_Sis.Controllers
{
    [RoleAuthorize("administrador")]
    public class AgenciasController : BaseController
    {
        private readonly ApuestasDataContext _context;

        public AgenciasController(ApuestasDataContext context)
        {
            _context = context;
        }

        // GET: Agencias
        public async Task<IActionResult> Index()
        {
            return View(await _context.Agencias.ToListAsync());
        }

        // GET: Agencias/Create
        public IActionResult Create()
        {
            // Opcional: predefinir valores
            return View(new Agencia
            {
                Activo = true,
                FechaRegistro = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Agencia agencia)
        {
            ModelState.Remove(nameof(Agencia.FechaRegistro));

            if (!ModelState.IsValid)
                return View(agencia);

            agencia.AgenciaId = Guid.NewGuid();
            agencia.Nombre = agencia.Nombre?.Trim() ?? "";
            agencia.Direccion = agencia.Direccion?.Trim() ?? "";
            agencia.FechaRegistro = DateTime.Now;

            _context.Add(agencia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Agencias/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agencia = await _context.Agencias.FindAsync(id);
            if (agencia == null)
            {
                return NotFound();
            }
            return View(agencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Agencia agencia)
        {
            if (id != agencia.AgenciaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agencia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgenciaExists(agencia.AgenciaId))
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
            return View(agencia);
        }

        // GET: Agencias/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agencia = await _context.Agencias
                .FirstOrDefaultAsync(m => m.AgenciaId == id);
            if (agencia == null)
            {
                return NotFound();
            }

            return View(agencia);
        }

        // POST: Agencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var agencia = await _context.Agencias.FindAsync(id);
            if (agencia != null)
            {
                _context.Agencias.Remove(agencia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgenciaExists(Guid id)
        {
            return _context.Agencias.Any(e => e.AgenciaId == id);
        }
    }
}