using Microsoft.EntityFrameworkCore;
using Vendas.Business.Interface;
using Vendas.Models;

namespace Vendas.Business
{
    public class ProcessData : IProcessData
    {
        private readonly IProdutoBusiness _produtoBusiness;

        public ProcessData(IProdutoBusiness produtoBusiness)
        {
            _produtoBusiness = produtoBusiness;
        }

        public void ProcessUpdate(Produto produtoEnviado)
        {
            _produtoBusiness.ProcessarAtualizacao(produtoEnviado);
        }

        public void ProcessCreate(Produto produto)
        {
            _produtoBusiness.ProcessarCriacao(produto);
        }
    }
}
