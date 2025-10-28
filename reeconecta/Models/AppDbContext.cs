using Microsoft.EntityFrameworkCore;

namespace reeconecta.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Produto> Produtos { get; set; }
    }
}
