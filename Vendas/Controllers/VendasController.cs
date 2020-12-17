using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Vendas.Business.Interface;
using Vendas.Models;
using Vendas.Repository.Interface;
using Vendas.Servicos.Interface;

namespace Vendas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendasController : ControllerBase
    {
        private readonly IProdutoBusiness _produtoBusiness;
        private readonly IProdutoMessageServices _produtoMessageServices;

        public VendasController(IProdutoBusiness produtoBusiness, IConfiguration _configuration, IProdutoMessageServices produtoMessageServices)
        {
            _produtoBusiness= produtoBusiness;
            _produtoMessageServices = produtoMessageServices;
        }

        // GET: api/Vendas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _produtoBusiness.GetAll().Where(x => x.Quantidade > 0).ToListAsync();
        }

        // POST: api/Vendas
        [HttpPost]
        public ActionResult<Produto> RealizarVenda(int idProduto, int quantidade)
        {
            if (quantidade <= 0)
                return new ContentResult() { Content = "Quantidade informada para a venda é inválida", StatusCode = 200 };

            var produto = _produtoBusiness.GetProduto(idProduto);

            if (produto == null)
            {
                return NotFound();
            }

            if (produto.Quantidade < quantidade)
                return new ContentResult() { Content = "Quantidade de produto em estoque é insuficiente para a venda", StatusCode = 200 };

            produto.Quantidade -= quantidade;

            try
            {
                _produtoBusiness.Update(produto);
                var produtoVendido = new ProdutoVendido() { Id = produto.Id, Quantidade = quantidade };
                _produtoMessageServices.EnviarMensagemProdutoVendido(produtoVendido);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_produtoBusiness.ProdutoExiste(idProduto))
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
    }
}
