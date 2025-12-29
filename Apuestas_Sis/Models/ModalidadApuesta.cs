using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.Models;

public class ModalidadApuesta
{
    [Key]
    public Guid ModalidadApuestaId { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "La cantidad de cifras es obligatoria.")]
    [Range(2, 4, ErrorMessage = "Las cifras permitidas son 2, 3 o 4.")]
    [Display(Name = "Cifras")]
    public int Cifras { get; set; } // 2, 3, 4

    [StringLength(120, ErrorMessage = "La descripción no puede superar los 120 caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Fecha de creación")]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}