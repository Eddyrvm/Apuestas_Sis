using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Apuestas_Sis.Models;

public class UsuarioRol
{
    [Key]
    public Guid UsuarioRolId { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "El usuario es obligatorio.")]
    public Guid UsuarioId { get; set; }

    // null si EsGlobal=true
    public Guid? AgenciaId { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    public Guid RolId { get; set; }

    public bool EsGlobal { get; set; } = false;
    public bool Activo { get; set; } = true;

    public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    public DateTime? FechaDesactivacion { get; set; }

    [ValidateNever]
    public Usuario Usuario { get; set; } = default!;

    [ValidateNever]
    public Agencia? Agencia { get; set; }

    [ValidateNever]
    public Rol Rol { get; set; } = default!;
}