using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Vendas.Helpers;
using Vendas.Models;
using Vendas.Repository.Interface;

namespace Vendas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendasController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;
        private const string endpointServiceBus = "Endpoint=sb://aplicacoesdistribuidascalixto.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=k/7AGJ+SvFXj75fa/BvqU9F90788XAtVMdsCJ53oa9E=";

        public VendasController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        // GET: api/Vendas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _produtoRepository.GetAll().Where(x => x.Quantidade > 0).ToListAsync();
        }

        // POST: api/Vendas
        [HttpPost]
        public async Task<ActionResult<Produto>> RealizarVenda(int idProduto, int quantidade)
        {
            if (!ProdutoExists(idProduto))
            {
                return NotFound();
            }

            Produto produto = GetProduto(idProduto);

            if (produto.Quantidade < quantidade)
                return new ContentResult() { Content = "Quantidade insuficiente de produto para a venda", StatusCode = 200 };

            produto.Quantidade -= quantidade;

            try
            {
                _produtoRepository.Update(produto);

                var serviceBusTopicClient = new TopicClient(endpointServiceBus, "produtovendido");

                var message = new Message(produto.ToJsonBytes())
                {
                    ContentType = "application/json"
                };

                serviceBusTopicClient.SendAsync(message);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(idProduto))
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

        private bool ProdutoExists(int id)
        {
            return _produtoRepository.ProdutoExiste(id);
        }

        private Produto GetProduto(int id)
        {
            return _produtoRepository.GetById(id);
        }
    }
}
