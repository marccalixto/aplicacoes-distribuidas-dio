using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vendas.Business;
using Vendas.Business.Interface;
using Vendas.Helpers;
using Vendas.Models;
using Vendas.Repository.Interface;
using Vendas.Servicos.Interface;

namespace Vendas.Servicos
{
    public class ProdutoMessageServices : IProdutoMessageServices
    {
        private const string endpointServiceBus = "Endpoint=sb://aplicacoesdistribuidascalixto.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=k/7AGJ+SvFXj75fa/BvqU9F90788XAtVMdsCJ53oa9E=";
        //private readonly IProdutoBusiness _produtoBusiness;
        private readonly SubscriptionClient _serviceBusClientProdutoCriado;
        private readonly SubscriptionClient _serviceBusClientProdutoAtualizado;

        public ProdutoMessageServices(/*IProdutoBusiness produtoBusiness*/)
        {
            //_produtoBusiness = produtoBusiness;

            _serviceBusClientProdutoCriado = new SubscriptionClient(endpointServiceBus, "produtocriado", "produtocriadosubscricao");
            _serviceBusClientProdutoAtualizado = new SubscriptionClient(endpointServiceBus, "produtoatualizado", "produtoatualizadosubscricao");
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
            //_produtoBusiness.ProcessarAtualizacao(produtoEnviado);
            await _serviceBusClientProdutoAtualizado.CompleteAsync(message.SystemProperties.LockToken);
        }

        private async Task ProcessMessageProdutoCriadoAsync(Message message, CancellationToken arg2)
        {
            var produto = message.Body.ParseJson<Produto>();
            //_produtoBusiness.ProcessarCriacao(produto);
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

        public async Task CloseQueueAsync()
        {
            await _serviceBusClientProdutoAtualizado.CloseAsync();
            await _serviceBusClientProdutoCriado.CloseAsync();
        }
    }
}
