using System.Collections.Generic;
using System.Linq;

namespace Vendas.Repository.Interface
{
    public interface IBaseRepository<T>  where T : class
    {
        public IEnumerable<T> ListAll();
        public IQueryable<T> GetAll();
        public void Add(T item);
        public void Remove(T item);
        public void Update(T item);
    }
}
