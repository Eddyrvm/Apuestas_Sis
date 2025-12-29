using Apuestas_Sis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apuestas_Sis.Controllers
{
    public class ModalidadApuestasController : Controller
    {
        private readonly ApuestasDataContext _context;

        public ModalidadApuestasController(ApuestasDataContext context)
        {
            _context = context;
        }

        // GET: ModalidadApuestas
        public async Task<IActionResult> Index()
        {
            return View(await _context.ModalidadApuestas.ToListAsync());
        }

        // GET: ModalidadApuestas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modalidadApuesta = await _context.ModalidadApuestas
                .FirstOrDefaultAsync(m => m.ModalidadApuestaId == id);
            if (modalidadApuesta == null)
            {
                return NotFound();
            }

            return View(modalidadApuesta);
        }

        // GET: ModalidadApuestas/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ModalidadApuesta modalidadApuesta)
        {
            // Regla de negocio: solo 2, 3 o 4 cifras
            if (modalidadApuesta.Cifras < 2 || modalidadApuesta.Cifras > 4)
            {
                ModelState.AddModelError(nameof(modalidadApuesta.Cifras),
                    "La modalidad solo puede ser de 2, 3 o 4 cifras.");
            }

            // Normaliza descripción (opcional)
            if (!string.IsNullOrWhiteSpace(modalidadApuesta.Descripcion))
            {
                modalidadApuesta.Descripcion = modalidadApuesta.Descripcion.Trim();
            }

            // Evitar duplicados por Cifras
            bool existe = await _context.ModalidadApuestas
                .AnyAsync(x => x.Cifras == modalidadApuesta.Cifras);

            if (existe)
            {
                ModelState.AddModelError(nameof(modalidadApuesta.Cifras),
                    "Ya existe una modalidad registrada con esa cantidad de cifras.");
            }

            if (!ModelState.IsValid)
                return View(modalidadApuesta);

            modalidadApuesta.ModalidadApuestaId = Guid.NewGuid();
            modalidadApuesta.FechaCreacion = DateTime.Now;

            _context.Add(modalidadApuesta);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Modalidad creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: ModalidadApuestas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modalidadApuesta = await _context.ModalidadApuestas.FindAsync(id);
            if (modalidadApuesta == null)
            {
                return NotFound();
            }
            return View(modalidadApuesta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ModalidadApuesta input)
        {
            if (id != input.ModalidadApuestaId)
                return NotFound();

            // Regla de negocio: solo 2, 3 o 4 cifras
            if (input.Cifras < 2 || input.Cifras > 4)
            {
                ModelState.AddModelError(nameof(input.Cifras),
                    "La modalidad solo puede ser de 2, 3 o 4 cifras.");
            }

            // Normaliza descripción (opcional)
            if (!string.IsNullOrWhiteSpace(input.Descripcion))
            {
                input.Descripcion = input.Descripcion.Trim();
            }

            // Evitar duplicados por Cifras (excluye el mismo registro)
            bool existe = await _context.ModalidadApuestas
                .AnyAsync(x => x.Cifras == input.Cifras && x.ModalidadApuestaId != input.ModalidadApuestaId);

            if (existe)
            {
                ModelState.AddModelError(nameof(input.Cifras),
                    "Ya existe otra modalidad registrada con esa cantidad de cifras.");
            }

            if (!ModelState.IsValid)
                return View(input);

            var entity = await _context.ModalidadApuestas
                .FirstOrDefaultAsync(x => x.ModalidadApuestaId == id);

            if (entity == null)
                return NotFound();

            // Actualiza solo campos permitidos
            entity.Cifras = input.Cifras;
            entity.Descripcion = input.Descripcion;
            entity.Activo = input.Activo;

            // NO tocar entity.FechaCreacion

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Modalidad actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: ModalidadApuestas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modalidadApuesta = await _context.ModalidadApuestas
                .FirstOrDefaultAsync(m => m.ModalidadApuestaId == id);
            if (modalidadApuesta == null)
            {
                return NotFound();
            }

            return View(modalidadApuesta);
        }

        // POST: ModalidadApuestas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var modalidadApuesta = await _context.ModalidadApuestas.FindAsync(id);
            if (modalidadApuesta != null)
            {
                _context.ModalidadApuestas.Remove(modalidadApuesta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModalidadApuestaExists(Guid id)
        {
            return _context.ModalidadApuestas.Any(e => e.ModalidadApuestaId == id);
        }
    }
}