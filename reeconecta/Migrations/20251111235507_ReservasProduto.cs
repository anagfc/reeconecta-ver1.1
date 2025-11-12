using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reeconecta.Migrations
{
    /// <inheritdoc />
    public partial class ReservasProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoInteressado_Produtos_ProdutoId",
                table: "ProdutoInteressado");

            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoInteressado_Usuarios_UsuarioId",
                table: "ProdutoInteressado");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Usuarios_AnuncianteId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservaProduto_Produtos_ProdutoId",
                table: "ReservaProduto");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservaProduto_Usuarios_UsuarioId",
                table: "ReservaProduto");

            migrationBuilder.DropForeignKey(
                name: "FK_VisualizacaoProduto_Produtos_ProdutoId",
                table: "VisualizacaoProduto");

            migrationBuilder.DropForeignKey(
                name: "FK_VisualizacaoProduto_Usuarios_UsuarioId",
                table: "VisualizacaoProduto");

           

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoInteressado_Produtos_ProdutoId",
                table: "ProdutoInteressado",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoInteressado_Usuarios_UsuarioId",
                table: "ProdutoInteressado",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Usuarios_AnuncianteId",
                table: "Produtos",
                column: "AnuncianteId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservaProduto_Produtos_ProdutoId",
                table: "ReservaProduto",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservaProduto_Usuarios_UsuarioId",
                table: "ReservaProduto",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisualizacaoProduto_Produtos_ProdutoId",
                table: "VisualizacaoProduto",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisualizacaoProduto_Usuarios_UsuarioId",
                table: "VisualizacaoProduto",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoInteressado_Produtos_ProdutoId",
                table: "ProdutoInteressado");

            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoInteressado_Usuarios_UsuarioId",
                table: "ProdutoInteressado");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Usuarios_AnuncianteId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservaProduto_Produtos_ProdutoId",
                table: "ReservaProduto");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservaProduto_Usuarios_UsuarioId",
                table: "ReservaProduto");

            migrationBuilder.DropForeignKey(
                name: "FK_VisualizacaoProduto_Produtos_ProdutoId",
                table: "VisualizacaoProduto");

            migrationBuilder.DropForeignKey(
                name: "FK_VisualizacaoProduto_Usuarios_UsuarioId",
                table: "VisualizacaoProduto");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Produtos",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoInteressado_Produtos_ProdutoId",
                table: "ProdutoInteressado",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoInteressado_Usuarios_UsuarioId",
                table: "ProdutoInteressado",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Usuarios_AnuncianteId",
                table: "Produtos",
                column: "AnuncianteId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservaProduto_Produtos_ProdutoId",
                table: "ReservaProduto",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservaProduto_Usuarios_UsuarioId",
                table: "ReservaProduto",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisualizacaoProduto_Produtos_ProdutoId",
                table: "VisualizacaoProduto",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisualizacaoProduto_Usuarios_UsuarioId",
                table: "VisualizacaoProduto",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
