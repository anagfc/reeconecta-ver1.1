using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reeconecta.Migrations
{
    /// <inheritdoc />
    public partial class M03AddProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ContaAtiva",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AnuncioAtivo",
                table: "Produtos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContaAtiva",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "AnuncioAtivo",
                table: "Produtos");
        }
    }
}
