using Estoque.Models;
using Estoque.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Estoque.Repository
{
    public class ProdutoRepository : BaseRepository<Produto, ProdutoRepository>, IProdutoRepository
    {
        public ProdutoRepository(DbContextOptions<ProdutoRepository> options)
          : base(options)
        {
        }

        public Produto GetById(int id)
        {
            return this.Items.FirstOrDefault(x => x.Id == id);
        }
        public bool ProdutoExiste(int id)
        {
            return this.Items.Any(e => e.Id == id);
        }
    }
}
