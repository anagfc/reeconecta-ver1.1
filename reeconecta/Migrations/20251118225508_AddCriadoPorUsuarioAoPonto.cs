using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reeconecta.Migrations
{
    /// <inheritdoc />
    public partial class AddCriadoPorUsuarioAoPonto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CriadoPorUsuarioId",
                table: "Pontos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Pontos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Pontos_CriadoPorUsuarioId",
                table: "Pontos",
                column: "CriadoPorUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pontos_Usuarios_CriadoPorUsuarioId",
                table: "Pontos",
                column: "CriadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pontos_Usuarios_CriadoPorUsuarioId",
                table: "Pontos");

            migrationBuilder.DropIndex(
                name: "IX_Pontos_CriadoPorUsuarioId",
                table: "Pontos");

            migrationBuilder.DropColumn(
                name: "CriadoPorUsuarioId",
                table: "Pontos");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Pontos");
        }
    }
}