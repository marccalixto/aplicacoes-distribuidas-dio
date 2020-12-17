using System.Linq;
using Vendas.Models;

namespace Vendas.Business.Interface
{
    public interface IProdutoBusiness
    {
        void ProcessarAtualizacao(Produto produtoEnviado);
        void ProcessarCriacao(Produto produto);

        #region Buscas no repositório
        IQueryable<Produto> GetAll();
        void Update(Produto produto);
        bool ProdutoExiste(int idProduto);
        Produto GetProduto(int idProduto);
        #endregion
    }
}
