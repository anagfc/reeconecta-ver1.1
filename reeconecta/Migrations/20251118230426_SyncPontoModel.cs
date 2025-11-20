using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reeconecta.Migrations
{
    /// <inheritdoc />
    public partial class SyncPontoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pontos_Usuarios_CriadoPorUsuarioId",
                table: "Pontos");

            migrationBuilder.AlterColumn<int>(
                name: "CriadoPorUsuarioId",
                table: "Pontos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Pontos_Usuarios_CriadoPorUsuarioId",
                table: "Pontos",
                column: "CriadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pontos_Usuarios_CriadoPorUsuarioId",
                table: "Pontos");

            migrationBuilder.AlterColumn<int>(
                name: "CriadoPorUsuarioId",
                table: "Pontos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pontos_Usuarios_CriadoPorUsuarioId",
                table: "Pontos",
                column: "CriadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
