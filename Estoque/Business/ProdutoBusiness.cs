using Estoque.Business.Interface;
using Estoque.Models;
using Estoque.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Estoque.Business
{
    public class ProdutoBusiness : IProdutoBusiness
    {
        private IProdutoRepository _produtoRepository;

        public ProdutoBusiness(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public void Add(Produto produto)
        {
            _produtoRepository.Add(produto);
        }

        public IQueryable<Produto> GetAll()
        {
            return _produtoRepository.GetAll();
        }

        public Produto GetById(int idProduto)
        {
            return _produtoRepository.GetById(idProduto);
        }

        public bool ProdutoExiste(int idProduto)
        {
            return _produtoRepository.GetAll().Any(x => x.Id == idProduto);
        }

        public void ProcessarVenda(ProdutoVendido produtoVendido)
        {
            if (_produtoRepository.ProdutoExiste(produtoVendido.Id))
            {
                Produto produto = _produtoRepository.GetById(produtoVendido.Id);

                produto.Quantidade -= produtoVendido.Quantidade;

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

        public void Update(Produto produto)
        {
            _produtoRepository.Update(produto);
        }

        public void ValidarProduto(Produto produto)
        {
            if (ProdutoExistsComCodigoOuNome(produto.Id, produto.CodigoProduto, produto.Nome))
                throw new Exception("Já existe um produto com o mesmo código ou nome");

            if (produto.Preco == 0)
                throw new Exception("Preço do produto não pode ser R$ 0,00");

            if (produto.Quantidade == 0)
                throw new Exception("Quantidade do produto não pode ser 0");
        }

        private bool ProdutoExistsComCodigoOuNome(int idProduto, string codigoProduto, string nomeProduto)
        {
            codigoProduto = codigoProduto.ToLower();
            nomeProduto = nomeProduto.ToLower();

            return _produtoRepository.GetAll().Any(e => e.Id != idProduto && (e.CodigoProduto.ToLower() == codigoProduto || e.Nome.ToLower() == nomeProduto));
        }
    }
}
