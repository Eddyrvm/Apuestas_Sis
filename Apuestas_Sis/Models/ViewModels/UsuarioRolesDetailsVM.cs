namespace Apuestas_Sis.Models.ViewModels;

public class UsuarioRolesDetailsVM
{
    public Guid UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;

    public List<UsuarioRol> Roles { get; set; } = new();
}