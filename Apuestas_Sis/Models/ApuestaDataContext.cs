using Microsoft.EntityFrameworkCore;

namespace Apuestas_Sis.Models;

public class ApuestasDataContext : DbContext
{
    public ApuestasDataContext(DbContextOptions<ApuestasDataContext> options)
        : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; } = default!;
    public DbSet<Agencia> Agencias { get; set; } = default!;
    public DbSet<Rol> Roles { get; set; } = default!;
    public DbSet<UsuarioAgencia> UsuarioAgencias { get; set; } = default!;
    public DbSet<UsuarioRol> UsuarioRoles { get; set; } = default!;

    public DbSet<TipoJuego> TipoJuego { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // LLAVES
        // =========================

        // UsuarioAgencia: PK compuesta (ok)
        modelBuilder.Entity<UsuarioAgencia>()
            .HasKey(x => new { x.UsuarioId, x.AgenciaId });

        // UsuarioRol: PK por Id surrogate (recomendado)
        modelBuilder.Entity<UsuarioRol>()
            .HasKey(x => x.UsuarioRolId);

        // =========================
        // RELACIONES (RESTRICT)
        // =========================
        modelBuilder.Entity<UsuarioAgencia>()
            .HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UsuarioAgencia>()
            .HasOne(x => x.Agencia)
            .WithMany()
            .HasForeignKey(x => x.AgenciaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UsuarioRol>()
            .HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UsuarioRol>()
            .HasOne(x => x.Rol)
            .WithMany()
            .HasForeignKey(x => x.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UsuarioRol>()
            .HasOne(x => x.Agencia)
            .WithMany()
            .HasForeignKey(x => x.AgenciaId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // ÍNDICES ÚTILES
        // =========================
        modelBuilder.Entity<Usuario>()
            .HasIndex(x => x.UsuarioLogin)
            .IsUnique();

        modelBuilder.Entity<Rol>()
            .HasIndex(x => x.Nombre)
            .IsUnique();

        modelBuilder.Entity<Agencia>()
            .HasIndex(x => x.Nombre)
            .IsUnique();

        // Consultas rápidas
        modelBuilder.Entity<UsuarioAgencia>()
            .HasIndex(x => new { x.UsuarioId, x.Activo });

        modelBuilder.Entity<UsuarioRol>()
            .HasIndex(x => new { x.UsuarioId, x.Activo, x.EsGlobal });

        modelBuilder.Entity<UsuarioRol>()
            .HasIndex(x => new { x.UsuarioId, x.AgenciaId, x.Activo });

        // =========================
        // ÚNICOS (EVITAR DUPLICADOS)
        // =========================

        // 1) Evita duplicar el mismo rol para el mismo usuario en la misma agencia
        // (solo aplica cuando AgenciaId NO es NULL)
        modelBuilder.Entity<UsuarioRol>()
            .HasIndex(x => new { x.UsuarioId, x.AgenciaId, x.RolId })
            .IsUnique()
            .HasFilter("[AgenciaId] IS NOT NULL");

        // 2) Evita duplicar el mismo rol global para el mismo usuario
        // (solo aplica cuando AgenciaId es NULL y EsGlobal=1)
        modelBuilder.Entity<UsuarioRol>()
            .HasIndex(x => new { x.UsuarioId, x.RolId, x.EsGlobal })
            .IsUnique()
            .HasFilter("[AgenciaId] IS NULL AND [EsGlobal] = 1");
    }
}