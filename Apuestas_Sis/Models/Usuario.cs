using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Usuario
    {
        [Key]
        public Guid UsuarioId { get; set; } = Guid.NewGuid();

        // Identificación y acceso
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede superar los 50 caracteres.")]
        [Display(Name = "Usuario")]
        public string UsuarioLogin { get; set; } = default!; // Ej: admin, vendedor01

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [Display(Name = "Contraseña")]
        public string PasswordHash { get; set; } = default!; // Hash (BCrypt / SHA256)

        // Datos informativos
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los nombres no pueden superar los 100 caracteres.")]
        public string Nombres { get; set; } = default!;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres.")]
        public string Apellidos { get; set; } = default!;

        [StringLength(10, ErrorMessage = "La cédula no puede superar los 10 dígitos.")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = default!;

        [StringLength(15, ErrorMessage = "La cédula no puede superar los 15 dígitos.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = default!;

        // Estado del usuario
        [Display(Name = "Usuario activo")]
        public bool Activo { get; set; } = true;

        // Auditoría
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}