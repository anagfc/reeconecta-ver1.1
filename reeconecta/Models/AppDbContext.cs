using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace reeconecta.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<ProdutoInteressado> ProdutosInteressado { get; set; }

        public DbSet<ReservaProduto> ReservasProduto { get; set; }

        public DbSet<VisualizacaoProduto> VisualizacaoProdutos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Produtos)
                .HasForeignKey(p => p.AnuncianteId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ReservaProduto>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.ReservasProduto)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ReservaProduto>()
                .HasOne(r => r.Produto)
                .WithMany(p => p.ReservasProduto)
                .HasForeignKey(r => r.ProdutoId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<PontoDeColeta> PontosDeColeta { get; set; }
    }
}