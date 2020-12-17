using System.Threading.Tasks;
using Vendas.Models;

namespace Vendas.Servicos.Interface
{
    public interface IProdutoMessageServices
    {
        void RegisterOnMessageHandlerAndReceiveMessagesProdutoCriado();
        void RegisterOnMessageHandlerAndReceiveMessagesProdutoAtualizado();
        void EnviarMensagemProdutoVendido(ProdutoVendido produtoVendido);
        Task CloseQueueAsync();
    }
}
