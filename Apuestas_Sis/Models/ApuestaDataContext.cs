using Microsoft.EntityFrameworkCore;

namespace Apuestas_Sis.Models;

public class ApuestasDataContext : DbContext
{
    public ApuestasDataContext(DbContextOptions<ApuestasDataContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; } = default!;
    public DbSet<Agencia> Agencias { get; set; } = default!;
    public DbSet<Rol> Roles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Sin relaciones por ahora
    }
}