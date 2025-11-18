using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reeconecta.Migrations
{
    /// <inheritdoc />
    public partial class ImagensParaPontos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Pontos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Pontos");
        }
    }
}
