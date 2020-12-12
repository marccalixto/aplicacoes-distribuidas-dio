using Vendas.Models;

namespace Vendas.Business.Interface
{
    public interface IProdutoBusiness
    {
        void ProcessarAtualizacao(Produto produtoEnviado);
        void ProcessarCriacao(Produto produto);
    }
}
