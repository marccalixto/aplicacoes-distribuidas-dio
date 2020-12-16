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
using Estoque.Servicos.Interface;
using Estoque.Repository.Interface;
using Estoque.Business.Interface;

namespace Estoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoBusiness _produtoBusiness;
        private readonly IProdutoMessageServices _produtoMessageServices;

        public ProdutosController(IProdutoBusiness produtoBusiness, IProdutoMessageServices produtoMessageServices)
        {
            _produtoBusiness = produtoBusiness;
            _produtoMessageServices = produtoMessageServices;
        }

        // GET: api/Produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _produtoBusiness.GetAll().ToListAsync();
        }

        // PUT: api/Produtos/5
        [HttpPut("{id}")]
        public IActionResult AtualizarProduto(int id, Produto produto)
        {
            if (id != produto.Id)
            {
                return BadRequest();
            }

            try
            {
                _produtoBusiness.ValidarProduto(produto);
                _produtoBusiness.Update(produto);

                _produtoMessageServices.EnviarMensagemProdutoAtualizado(produto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_produtoBusiness.ProdutoExiste(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return new ContentResult() { Content = ex.Message }; ;
            }

            return NoContent();
        }

        // POST: api/Produtos
        [HttpPost]
        public ActionResult<Produto> CriarProduto(Produto produto)
        {
            try
            {
                _produtoBusiness.ValidarProduto(produto);
                _produtoBusiness.Add(produto);

                _produtoMessageServices.EnviarMensagemProdutoCriado(produto);
            }
            catch (Exception ex)
            {
                return new ContentResult() { Content = ex.Message }; ;
            }

            return produto;
        }
    }
}
