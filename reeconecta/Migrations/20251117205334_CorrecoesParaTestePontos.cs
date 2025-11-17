using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reeconecta.Migrations
{
    /// <inheritdoc />
    public partial class CorrecoesParaTestePontos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FuncionamentoPonto",
                table: "Pontos");

            migrationBuilder.DropColumn(
                name: "HorarioPonto",
                table: "Pontos");

            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Pontos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FuncionamentoPonto",
                table: "Pontos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "HorarioPonto",
                table: "Pontos",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Pontos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
