using Estoque.Models;
using System.Linq;

namespace Estoque.Business.Interface
{
    public interface IProdutoBusiness
    {
        void ProcessarVenda(ProdutoVendido produtoVendido);
        IQueryable<Produto> GetAll();
        void Update(Produto produto);
        void Add(Produto produto);
        Produto GetById(int idProduto);
        bool ProdutoExiste(int idProduto);
        void ValidarProduto(Produto produto);
    }
}
