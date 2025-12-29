using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.Models;

public class TipoJuego
{
    [Key]
    public Guid TipoJuegoId { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "El nombre del tipo de juego es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre no puede exceder los 80 caracteres.")]
    public string Nombre { get; set; } = default!;

    [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres.")]
    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    [Required(ErrorMessage = "La fecha de creación es obligatoria.")]
    public DateTime FechaCreacion { get; set; }
}