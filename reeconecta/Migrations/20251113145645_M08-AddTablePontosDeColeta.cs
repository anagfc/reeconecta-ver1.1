using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reeconecta.Migrations
{
    /// <inheritdoc />
    public partial class M08AddTablePontosDeColeta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pontos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomePonto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DescricaoPonto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CepPonto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuncionamentoPonto = table.Column<int>(type: "int", nullable: false),
                    HorarioPonto = table.Column<TimeOnly>(type: "time", nullable: false),
                    TelefoneP01 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WppTelP1 = table.Column<bool>(type: "bit", nullable: false),
                    TelefoneP02 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WppTelP2 = table.Column<bool>(type: "bit", nullable: false),
                    Imagem = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pontos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pontos");
        }
    }
}
