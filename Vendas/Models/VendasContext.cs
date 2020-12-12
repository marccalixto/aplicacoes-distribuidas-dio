using Microsoft.EntityFrameworkCore;

namespace Vendas.Models
{
    public class VendasContext : DbContext
    {
        public VendasContext(DbContextOptions<VendasContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
    }
}
