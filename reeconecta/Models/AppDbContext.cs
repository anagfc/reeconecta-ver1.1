using Microsoft.EntityFrameworkCore;

namespace reeconecta.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Produto> Produtos { get; set; }

        public DbSet<ProdutoInteressado> ProdutoInteressados { get; set; }

        public DbSet<ReservaProduto> ReservaProdutos { get; set; }

        public DbSet<VisualizacaoProduto> VisualizacaoProdutos { get; set; }

    }
}
