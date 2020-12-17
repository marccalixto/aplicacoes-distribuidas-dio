using Estoque.Models;

namespace Estoque.Business.Interface
{
    public interface IProcessData
    {
        void ProcessUpdate(ProdutoVendido produtoVendido);
    }
}
