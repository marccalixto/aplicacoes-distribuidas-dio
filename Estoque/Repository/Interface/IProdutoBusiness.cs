using Estoque.Models;

namespace Estoque.Repository.Interface
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        Produto GetById(int id);

        bool ProdutoExiste(int id);
    }
}
