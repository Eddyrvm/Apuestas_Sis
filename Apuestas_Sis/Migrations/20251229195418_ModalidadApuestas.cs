using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apuestas_Sis.Migrations
{
    /// <inheritdoc />
    public partial class ModalidadApuestas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "TipoJuego",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateTable(
                name: "ModalidadApuestas",
                columns: table => new
                {
                    ModalidadApuestaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cifras = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModalidadApuestas", x => x.ModalidadApuestaId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModalidadApuestas");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "TipoJuego",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
