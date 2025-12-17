using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.Models;

public class UsuarioAgencia
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    public Guid UsuarioId { get; set; }

    [Required(ErrorMessage = "La agencia es obligatoria.")]
    public Guid AgenciaId { get; set; }

    public bool Activo { get; set; } = true;
    public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    public DateTime? FechaDesactivacion { get; set; }

    [ValidateNever]
    public Usuario Usuario { get; set; } = default!;

    [ValidateNever]
    public Agencia Agencia { get; set; } = default!;
}