using Estoque.Business.Interface;
using Estoque.Helpers;
using Estoque.Models;
using Estoque.Servicos.Interface;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Estoque.Servicos
{
    public class ProdutoMessageServices : IProdutoMessageServices
    {
        private readonly string _endpointServiceBus;
        private readonly SubscriptionClient _serviceBusClientProdutoVendido;
        private readonly TopicClient _serviceBusTopicProdutoCriado;
        private readonly TopicClient _serviceBusTopicProdutoAtualizado;
        private readonly IServiceProvider _serviceProvider;

        public ProdutoMessageServices(IServiceProvider serviceProvider, IConfiguration _configuration)
        {
            _serviceProvider = serviceProvider;
            _endpointServiceBus = _configuration.GetConnectionString("EndpointServiceBusConnection");
            
            _serviceBusClientProdutoVendido = new SubscriptionClient(_endpointServiceBus, "produtovendido", "produtovendidosubscricao");
            _serviceBusTopicProdutoCriado = new TopicClient(_endpointServiceBus, "produtocriado");
            _serviceBusTopicProdutoAtualizado = new TopicClient(_endpointServiceBus, "produtoatualizado");
        }

        public void RegisterOnMessageHandlerAndReceiveMessagesProdutoVendido()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _serviceBusClientProdutoVendido.RegisterMessageHandler(ProcessMessageProdutoVendidoAsync, messageHandlerOptions);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            //_logger.LogError(exceptionReceivedEventArgs.Exception, "Message handler encountered an exception");
            //var context = arg.ExceptionReceivedContext;

            //_logger.LogDebug($"- Endpoint: {context.Endpoint}");
            //_logger.LogDebug($"- Entity Path: {context.EntityPath}");
            //_logger.LogDebug($"- Executing Action: {context.Action}");

            return Task.CompletedTask;

        }

        private async Task ProcessMessageProdutoVendidoAsync(Message message, CancellationToken arg2)
        {
            var produto = message.Body.ParseJson<Produto>();
            var scope = _serviceProvider.CreateScope();
            var _produtoBusiness = scope.ServiceProvider.GetService<IProdutoBusiness>();
            _produtoBusiness.ProcessarVenda(produto);
            await _serviceBusClientProdutoVendido.CompleteAsync(message.SystemProperties.LockToken);
        }

        public void EnviarMensagemProdutoAtualizado(Produto produto)
        {
            var message = new Message(produto.ToJsonBytes())
            {
                ContentType = "application/json"
            };

            _serviceBusTopicProdutoAtualizado.SendAsync(message);
        }

        public void EnviarMensagemProdutoCriado(Produto produto)
        {
            var message = new Message(produto.ToJsonBytes())
            {
                ContentType = "application/json"
            };

            _serviceBusTopicProdutoCriado.SendAsync(message);
        }

        public async Task CloseQueueAsync()
        {
            await _serviceBusClientProdutoVendido.CloseAsync();
            await _serviceBusTopicProdutoAtualizado.CloseAsync();
            await _serviceBusTopicProdutoCriado.CloseAsync();
        }
    }
}
