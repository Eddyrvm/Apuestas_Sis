using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.Models.ViewModels;

public class UsuarioCreateVm
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede superar los 50 caracteres.")]
    [Display(Name = "Usuario")]
    public string UsuarioLogin { get; set; } = default!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = default!;

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100)]
    public string Nombres { get; set; } = default!;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    public string Apellidos { get; set; } = default!;

    [StringLength(10)]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = default!;

    [StringLength(15)]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = default!;

    [Display(Name = "Usuario activo")]
    public bool Activo { get; set; } = true;
}