using Vendas.Models;

namespace Vendas.Repository.Interface
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        Produto GetById(int id);

        bool ProdutoExiste(int id);
    }
}
