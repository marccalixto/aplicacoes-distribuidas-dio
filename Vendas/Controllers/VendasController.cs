using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Vendas.Models;
using Vendas.Repository.Interface;
using Vendas.Servicos.Interface;

namespace Vendas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendasController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoMessageServices _produtoMessageServices;

        public VendasController(IProdutoRepository produtoRepository, IConfiguration _configuration, IProdutoMessageServices produtoMessageServices)
        {
            _produtoRepository = produtoRepository;
            _produtoMessageServices = produtoMessageServices;
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

                _produtoMessageServices.EnviarMensagemProdutoVendido(produto);
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
