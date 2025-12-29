using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apuestas_Sis.Migrations
{
    /// <inheritdoc />
    public partial class Sorteo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sorteos",
                columns: table => new
                {
                    SorteoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoJuegoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModalidadApuestaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FechaSorteo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreadoPorUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sorteos", x => x.SorteoId);
                    table.ForeignKey(
                        name: "FK_Sorteos_ModalidadApuestas_ModalidadApuestaId",
                        column: x => x.ModalidadApuestaId,
                        principalTable: "ModalidadApuestas",
                        principalColumn: "ModalidadApuestaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sorteos_TipoJuego_TipoJuegoId",
                        column: x => x.TipoJuegoId,
                        principalTable: "TipoJuego",
                        principalColumn: "TipoJuegoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sorteos_Usuarios_CreadoPorUsuarioId",
                        column: x => x.CreadoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sorteos_CreadoPorUsuarioId",
                table: "Sorteos",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Sorteos_ModalidadApuestaId",
                table: "Sorteos",
                column: "ModalidadApuestaId");

            migrationBuilder.CreateIndex(
                name: "IX_Sorteos_TipoJuegoId",
                table: "Sorteos",
                column: "TipoJuegoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sorteos");
        }
    }
}
