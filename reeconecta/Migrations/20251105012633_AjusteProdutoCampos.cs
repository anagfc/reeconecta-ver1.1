using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reeconecta.Migrations
{
    /// <inheritdoc />
    public partial class AjusteProdutoCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Usuarios_AnuncianteId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "CriacaoAnuncio",
                table: "Produtos");

            migrationBuilder.RenameColumn(
                name: "StatusAnuncio",
                table: "Produtos",
                newName: "StatusProduto");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Produtos",
                newName: "Cidade");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CriacaoProduto",
                table: "Produtos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Produtos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "Produtos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProdutoInteressado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    DataInteresse = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoInteressado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdutoInteressado_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProdutoInteressado_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservaProduto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    DataReserva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservaProduto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservaProduto_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservaProduto_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisualizacaoProduto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    DataVisualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualizacaoProduto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisualizacaoProduto_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisualizacaoProduto_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoInteressado_ProdutoId",
                table: "ProdutoInteressado",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoInteressado_UsuarioId",
                table: "ProdutoInteressado",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaProduto_ProdutoId",
                table: "ReservaProduto",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaProduto_UsuarioId",
                table: "ReservaProduto",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualizacaoProduto_ProdutoId",
                table: "VisualizacaoProduto",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualizacaoProduto_UsuarioId",
                table: "VisualizacaoProduto",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Usuarios_AnuncianteId",
                table: "Produtos",
                column: "AnuncianteId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Usuarios_AnuncianteId",
                table: "Produtos");

            migrationBuilder.DropTable(
                name: "ProdutoInteressado");

            migrationBuilder.DropTable(
                name: "ReservaProduto");

            migrationBuilder.DropTable(
                name: "VisualizacaoProduto");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "CriacaoProduto",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "Produtos");

            migrationBuilder.RenameColumn(
                name: "StatusProduto",
                table: "Produtos",
                newName: "StatusAnuncio");

            migrationBuilder.RenameColumn(
                name: "Cidade",
                table: "Produtos",
                newName: "Nome");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CriacaoAnuncio",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Usuarios_AnuncianteId",
                table: "Produtos",
                column: "AnuncianteId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
