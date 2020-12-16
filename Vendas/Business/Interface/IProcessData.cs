using Vendas.Models;

namespace Vendas.Business.Interface
{
    public interface IProcessData
    {
        void ProcessUpdate(Produto produtoEnviado);
        void ProcessCreate(Produto produto);
    }
}
