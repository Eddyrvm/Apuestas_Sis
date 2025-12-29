using Apuestas_Sis.Filters;
using Apuestas_Sis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apuestas_Sis.Controllers;

[RoleAuthorize("administrador")]
public class TipoJuegosController : BaseController
{
    private readonly ApuestasDataContext _context;

    public TipoJuegosController(ApuestasDataContext context)
    {
        _context = context;
    }

    // GET: TipoJuegos
    public async Task<IActionResult> Index()
    {
        return View(await _context.TipoJuego.ToListAsync());
    }

    // GET: TipoJuegos/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TipoJuego model)
    {
        // Como no viene del formulario, quítalo del ModelState antes de validar
        ModelState.Remove(nameof(TipoJuego.FechaCreacion));

        if (!ModelState.IsValid)
            return View(model);

        // Asignación en servidor
        model.TipoJuegoId = Guid.NewGuid();
        model.FechaCreacion = DateTime.Now;

        _context.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: TipoJuegos/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tipoJuego = await _context.TipoJuego.FindAsync(id);
        if (tipoJuego == null)
        {
            return NotFound();
        }
        return View(tipoJuego);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TipoJuego model)
    {
        if (id != model.TipoJuegoId)
            return NotFound();

        // No viene del form
        ModelState.Remove(nameof(TipoJuego.FechaCreacion));

        if (!ModelState.IsValid)
            return View(model);

        var entity = await _context.TipoJuego.FindAsync(id);
        if (entity == null)
            return NotFound();

        // Actualizar solo lo editable
        entity.Nombre = model.Nombre;
        entity.Descripcion = model.Descripcion;
        entity.Activo = model.Activo;

        // FechaCreacion NO se toca
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: TipoJuegos/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tipoJuego = await _context.TipoJuego
            .FirstOrDefaultAsync(m => m.TipoJuegoId == id);
        if (tipoJuego == null)
        {
            return NotFound();
        }

        return View(tipoJuego);
    }

    // POST: TipoJuegos/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.TipoJuego.FindAsync(id);

        if (entity == null)
            return NotFound();

        // Borrado lógico
        entity.Activo = false;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool TipoJuegoExists(Guid id)
    {
        return _context.TipoJuego.Any(e => e.TipoJuegoId == id);
    }
}