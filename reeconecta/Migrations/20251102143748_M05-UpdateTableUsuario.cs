using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reeconecta.Migrations
{
    /// <inheritdoc />
    public partial class M05UpdateTableUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TipoPerfil",
                table: "Usuarios",
                newName: "TipodePerfil");

            migrationBuilder.AddColumn<string>(
                name: "EmailRepresentante",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeFantasia",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RazaoSocial",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RepresentanteLegal",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TipoUsuario",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "WppTel1",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WppTel2",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailRepresentante",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "NomeFantasia",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RazaoSocial",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RepresentanteLegal",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TipoUsuario",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "WppTel1",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "WppTel2",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "TipodePerfil",
                table: "Usuarios",
                newName: "TipoPerfil");
        }
    }
}
