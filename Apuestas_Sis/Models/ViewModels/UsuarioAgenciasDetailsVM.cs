namespace Apuestas_Sis.Models.ViewModels;

public class UsuarioAgenciasDetailsVM
{
    public Guid UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;

    public List<UsuarioAgencia> Activas { get; set; } = new();
    public List<UsuarioAgencia> Inactivas { get; set; } = new();
}