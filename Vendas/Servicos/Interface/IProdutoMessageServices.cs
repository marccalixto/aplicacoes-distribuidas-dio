using System.Threading.Tasks;

namespace Vendas.Servicos.Interface
{
    public interface IProdutoMessageServices
    {
        void RegisterOnMessageHandlerAndReceiveMessagesProdutoCriado();
        void RegisterOnMessageHandlerAndReceiveMessagesProdutoAtualizado();
        Task CloseQueueAsync();
    }
}
