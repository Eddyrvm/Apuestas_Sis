using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.Models;

public class Sorteo
{
    [Key]
    public Guid SorteoId { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "El tipo de juego es obligatorio.")]
    public Guid TipoJuegoId { get; set; }

    [Required(ErrorMessage = "La modalidad (cifras) es obligatoria.")]
    public Guid ModalidadApuestaId { get; set; }

    [Required(ErrorMessage = "El nombre del sorteo es obligatorio.")]
    [StringLength(120, ErrorMessage = "El nombre no puede superar los 120 caracteres.")]
    public string Nombre { get; set; } = default!;

    [Required(ErrorMessage = "La fecha del sorteo es obligatoria.")]
    [Display(Name = "Fecha del sorteo")]
    public DateTime FechaSorteo { get; set; }

    [Required(ErrorMessage = "El estado del sorteo es obligatorio.")]
    [StringLength(20, ErrorMessage = "El estado no puede superar los 20 caracteres.")]
    public string Estado { get; set; } = "PROGRAMADO"; // PROGRAMADO, ABIERTO, CERRADO, SORTEADO, ANULADO

    public bool Activo { get; set; } = true;

    [StringLength(250, ErrorMessage = "La observación no puede superar los 250 caracteres.")]
    public string? Observacion { get; set; }

    // Auditoría: quién lo creó
    [Required(ErrorMessage = "El usuario creador es obligatorio.")]
    public Guid CreadoPorUsuarioId { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Navegación
    [ValidateNever] public TipoJuego TipoJuego { get; set; } = default!;

    [ValidateNever] public ModalidadApuesta ModalidadApuesta { get; set; } = default!;
    [ValidateNever] public Usuario CreadoPorUsuario { get; set; } = default!;
}