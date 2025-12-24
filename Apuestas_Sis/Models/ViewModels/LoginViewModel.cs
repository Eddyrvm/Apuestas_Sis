using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Usuario")]
    public string UsuarioLogin { get; set; } = default!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = default!;
}