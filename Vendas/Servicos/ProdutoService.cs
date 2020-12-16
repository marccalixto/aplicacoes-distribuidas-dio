using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Vendas.Models;
using Vendas.Servicos.Interface;

namespace Vendas.Servicos
{
    public class ProdutoService : IProdutoService
    {
        private readonly VendasContext _vendasContext;

        public ProdutoService()
        {
            _vendasContext = new VendasContext(new DbContextOptions<VendasContext>());
        }

        public IEnumerable<Produto> ListAll()
        {
            return _vendasContext.Produtos.ToArray();
        }

        public IQueryable<Produto> GetAll()
        {
            return _vendasContext.Produtos.AsQueryable();
        }

        public void Add(Produto item)
        {
            _vendasContext.Produtos.Add(item);
            _vendasContext.SaveChanges();
        }

        public void Remove(Produto produto)
        {
            _vendasContext.Remove(produto);
            _vendasContext.SaveChanges();
        }

        public void Update(Produto produto)
        {
            _vendasContext.Update(produto);
            _vendasContext.SaveChanges();
        }

        public Produto GetById(int id)
        {
            return _vendasContext.Produtos.FirstOrDefault(x => x.Id == id);
        }

        public bool ProdutoExiste(int id)
        {
            return _vendasContext.Produtos.Any(e => e.Id == id);
        }
    }
}
