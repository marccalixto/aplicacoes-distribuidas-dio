using Estoque.Business.Interface;
using Estoque.Models;

namespace Estoque.Business
{
    public class ProcessData : IProcessData
    {
        private readonly IProdutoBusiness _produtoBusiness;

        public ProcessData(IProdutoBusiness produtoBusiness)
        {
            _produtoBusiness = produtoBusiness;
        }

        public void ProcessUpdate(Produto produto)
        {
            _produtoBusiness.ProcessarVenda(produto);
        }
    }
}
