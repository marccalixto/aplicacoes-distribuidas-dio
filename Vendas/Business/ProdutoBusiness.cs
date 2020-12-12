using Microsoft.EntityFrameworkCore;
using Vendas.Business.Interface;
using Vendas.Models;
using Vendas.Repository.Interface;

namespace Vendas.Business
{
    public class ProdutoBusiness : IProdutoBusiness
    {
        private IProdutoRepository _produtoRepository;

        public ProdutoBusiness(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public void ProcessarAtualizacao(Produto produtoEnviado)
        {
            if (_produtoRepository.ProdutoExiste(produtoEnviado.Id))
            {
                Produto produto = _produtoRepository.GetById(produtoEnviado.Id);

                produto.CodigoProduto = produtoEnviado.CodigoProduto;
                produto.Nome = produtoEnviado.Nome;
                produto.Preco = produtoEnviado.Preco;
                produto.Quantidade = produtoEnviado.Quantidade;

                try
                {
                    _produtoRepository.Update(produto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
        }

        public void ProcessarCriacao(Produto produto)
        {
            _produtoRepository.Add(produto);
        }
    }
}
