using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apuestas_Sis.Migrations
{
    /// <inheritdoc />
    public partial class Add_UsuarioAgencia_UsuarioRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuarioAgencias",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDesactivacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioAgencias", x => new { x.UsuarioId, x.AgenciaId });
                    table.ForeignKey(
                        name: "FK_UsuarioAgencias_Agencias_AgenciaId",
                        column: x => x.AgenciaId,
                        principalTable: "Agencias",
                        principalColumn: "AgenciaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioAgencias_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRoles",
                columns: table => new
                {
                    UsuarioRolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EsGlobal = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDesactivacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRoles", x => x.UsuarioRolId);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Agencias_AgenciaId",
                        column: x => x.AgenciaId,
                        principalTable: "Agencias",
                        principalColumn: "AgenciaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_UsuarioLogin",
                table: "Usuarios",
                column: "UsuarioLogin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agencias_Nombre",
                table: "Agencias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAgencias_AgenciaId",
                table: "UsuarioAgencias",
                column: "AgenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAgencias_UsuarioId_Activo",
                table: "UsuarioAgencias",
                columns: new[] { "UsuarioId", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_AgenciaId",
                table: "UsuarioRoles",
                column: "AgenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_RolId",
                table: "UsuarioRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_UsuarioId_Activo_EsGlobal",
                table: "UsuarioRoles",
                columns: new[] { "UsuarioId", "Activo", "EsGlobal" });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_UsuarioId_AgenciaId_Activo",
                table: "UsuarioRoles",
                columns: new[] { "UsuarioId", "AgenciaId", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_UsuarioId_AgenciaId_RolId",
                table: "UsuarioRoles",
                columns: new[] { "UsuarioId", "AgenciaId", "RolId" },
                unique: true,
                filter: "[AgenciaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_UsuarioId_RolId_EsGlobal",
                table: "UsuarioRoles",
                columns: new[] { "UsuarioId", "RolId", "EsGlobal" },
                unique: true,
                filter: "[AgenciaId] IS NULL AND [EsGlobal] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioAgencias");

            migrationBuilder.DropTable(
                name: "UsuarioRoles");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_UsuarioLogin",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Nombre",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Agencias_Nombre",
                table: "Agencias");
        }
    }
}
