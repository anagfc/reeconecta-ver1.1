using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reeconecta.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePontoModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnderecoPonto",
                table: "Pontos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnderecoPonto",
                table: "Pontos");
        }
    }
}
