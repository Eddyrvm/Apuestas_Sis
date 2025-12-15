using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.Models
{
    public class Rol
    {
        [Key]
        public Guid RolId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(80, ErrorMessage = "El nombre del rol no puede superar los 80 caracteres.")]
        public string Nombre { get; set; } = default!;

        public bool Activo { get; set; } = true;
    }
}