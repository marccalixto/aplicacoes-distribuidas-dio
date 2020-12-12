using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Estoque.Models;
using Microsoft.Azure.ServiceBus;
using Estoque.Helpers;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace Estoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly EstoqueContext _context;
        private readonly string _endpointServiceBus;
      
        public ProdutosController(EstoqueContext context, IConfiguration _configuration)
        {
            _context = context;
            _endpointServiceBus = _configuration.GetConnectionString("EndpointServiceBusConnection");
            var serviceBusClientProdutoVendido= new SubscriptionClient(_endpointServiceBus, "produtovendido", "produtovendidosubscricao");

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            serviceBusClientProdutoVendido.RegisterMessageHandler(ProcessMessageProdutoVendidoAsync, messageHandlerOptions);
        }

        private Task ProcessMessageProdutoVendidoAsync(Message message, CancellationToken arg2)
        {
            var produtoVendidoEnviado = message.Body.ParseJson<ProdutoVendido>();

            if (ProdutoExists(produtoVendidoEnviado.Id))
            {
                Produto produto = GetProduto(produtoVendidoEnviado.Id);

                produto.Quantidade -= produtoVendidoEnviado.Quantidade;

                _context.Entry(produto).State = EntityState.Modified;

                try
                {
                    _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            return Task.CompletedTask;
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            //throw new NotImplementedException();
            return null;
        }

        // GET: api/Produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _context.Produtos.ToListAsync();
        }

        // PUT: api/Produtos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarProduto(int id, Produto produto)
        {
            if (id != produto.Id)
            {
                return BadRequest();
            }

            try
            {
                ValidarProduto(produto);
            }
            catch (Exception ex)
            {
                return new ContentResult() { Content = ex.Message }; ;
            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                var serviceBusTopicClient = new TopicClient(_endpointServiceBus, "produtoatualizado");

                var message = new Message(produto.ToJsonBytes())
                {
                    ContentType = "application/json"
                };

                serviceBusTopicClient.SendAsync(message);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private void ValidarProduto(Produto produto)
        {
            if (ProdutoExistsComCodigoOuNome(produto.CodigoProduto, produto.Nome))
                throw new Exception("Já existe um produto com o mesmo código ou nome");

            if (produto.Preco == 0)
                throw new Exception("Preço do produto não pode ser R$ 0,00");

            if (produto.Quantidade == 0)
                throw new Exception("Quantidade do produto não pode ser 0");
        }

        // POST: api/Produtos
        [HttpPost]
        public async Task<ActionResult<Produto>> CriarProduto(Produto produto)
        {
            try
            {
                ValidarProduto(produto);
            }
            catch (Exception ex)
            {
                return new ContentResult() { Content = ex.Message }; ;
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            var serviceBusTopicClient = new TopicClient(_endpointServiceBus, "produtocriado");

            var message = new Message(produto.ToJsonBytes())
            {
                ContentType = "application/json"
            };

            serviceBusTopicClient.SendAsync(message);

            return produto;
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
        private bool ProdutoExistsComCodigoOuNome(string codigoProduto, string nomeProduto)
        {
            codigoProduto = codigoProduto.ToLower();
            nomeProduto = nomeProduto.ToLower();

            return _context.Produtos.Any(e => e.CodigoProduto.ToLower() == codigoProduto || e.Nome.ToLower() == nomeProduto);
        }

        private Produto GetProduto(int id)
        {
            return _context.Produtos.FirstOrDefault(e => e.Id == id);
        }
    }
}
