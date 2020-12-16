using Estoque.Models;
using System.Threading.Tasks;

namespace Estoque.Servicos.Interface
{
    public interface IProdutoMessageServices
    {
        void RegisterOnMessageHandlerAndReceiveMessagesProdutoVendido();
        void EnviarMensagemProdutoCriado(Produto produto);
        void EnviarMensagemProdutoAtualizado(Produto produto);
        Task CloseQueueAsync();
    }
}
