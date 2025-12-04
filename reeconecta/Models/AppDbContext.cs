using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using reeconecta.Models;

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

        public DbSet<Ponto> Pontos { get; set; }

        public DbSet<Contato> Contatos { get; set; }

        public DbSet<Avaliacao> Avaliacoes { get; set; }


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

            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Ponto)
                .WithMany(p => p.Avaliacoes)
                .HasForeignKey(a => a.PontoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public DbSet<reeconecta.Models.Contato> Contato { get; set; } = default!;

    }
}