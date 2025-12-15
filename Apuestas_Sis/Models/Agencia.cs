using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.Models
{
    public class Agencia
    {
        [Key]
        public Guid AgenciaId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El nombre de la agencia es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre de la agencia no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = default!;

        [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
        public string? Direccion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}