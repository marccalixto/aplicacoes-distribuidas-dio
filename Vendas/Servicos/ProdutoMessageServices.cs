using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vendas.Business.Interface;
using Vendas.Helpers;
using Vendas.Models;
using Vendas.Servicos.Interface;

namespace Vendas.Servicos
{
    public class ProdutoMessageServices : IProdutoMessageServices
    {
        private readonly string _endpointServiceBus;
        private readonly SubscriptionClient _serviceBusClientProdutoCriado;
        private readonly SubscriptionClient _serviceBusClientProdutoAtualizado;
        private readonly TopicClient _serviceBusTopicProdutoVendido;
        private readonly IServiceProvider _serviceProvider;

        public ProdutoMessageServices(IServiceProvider serviceProvider, IConfiguration _configuration)
        {
            _serviceProvider = serviceProvider;
            _endpointServiceBus = _configuration.GetConnectionString("EndpointServiceBusConnection");

            _serviceBusClientProdutoCriado = new SubscriptionClient(_endpointServiceBus, "produtocriado", "produtocriadosubscricao");
            _serviceBusClientProdutoAtualizado = new SubscriptionClient(_endpointServiceBus, "produtoatualizado", "produtoatualizadosubscricao");
            _serviceBusTopicProdutoVendido = new TopicClient(_endpointServiceBus, "produtovendido");
        }

        public void RegisterOnMessageHandlerAndReceiveMessagesProdutoCriado()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _serviceBusClientProdutoCriado.RegisterMessageHandler(ProcessMessageProdutoCriadoAsync, messageHandlerOptions);
        }

        public void RegisterOnMessageHandlerAndReceiveMessagesProdutoAtualizado()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _serviceBusClientProdutoAtualizado.RegisterMessageHandler(ProcessMessageProdutoAtualizadoAsync, messageHandlerOptions);
        }

        private async Task ProcessMessageProdutoAtualizadoAsync(Message message, CancellationToken arg2)
        {
            var produtoEnviado = message.Body.ParseJson<Produto>();
            var scope = _serviceProvider.CreateScope();
            var _produtoBusiness = scope.ServiceProvider.GetService<IProdutoBusiness>();
            _produtoBusiness.ProcessarAtualizacao(produtoEnviado);
            await _serviceBusClientProdutoAtualizado.CompleteAsync(message.SystemProperties.LockToken);
        }

        private async Task ProcessMessageProdutoCriadoAsync(Message message, CancellationToken arg2)
        {
            var produto = message.Body.ParseJson<Produto>();
            var scope = _serviceProvider.CreateScope();
            var _produtoBusiness = scope.ServiceProvider.GetService<IProdutoBusiness>();
            _produtoBusiness.ProcessarCriacao(produto);
            await _serviceBusClientProdutoCriado.CompleteAsync(message.SystemProperties.LockToken);
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

        public void EnviarMensagemProdutoVendido(ProdutoVendido produtoVendido)
        {
            var message = new Message(produtoVendido.ToJsonBytes())
            {
                ContentType = "application/json"
            };

            _serviceBusTopicProdutoVendido.SendAsync(message);
        }

        public async Task CloseQueueAsync()
        {
            await _serviceBusClientProdutoAtualizado.CloseAsync();
            await _serviceBusClientProdutoCriado.CloseAsync();
        }
    }
}