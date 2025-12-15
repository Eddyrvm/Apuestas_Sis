using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.Models.ViewModels;

public class UsuarioEditVm
{
    [Required]
    public Guid UsuarioId { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Usuario")]
    public string UsuarioLogin { get; set; } = default!;

    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña (opcional)")]
    public string? NewPassword { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombres { get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string Apellidos { get; set; } = default!;

    [StringLength(10)]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = default!;

    [StringLength(15)]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = default!;

    [Display(Name = "Usuario activo")]
    public bool Activo { get; set; }

    [Display(Name = "Fecha de registro")]
    public DateTime FechaRegistro { get; set; }
}