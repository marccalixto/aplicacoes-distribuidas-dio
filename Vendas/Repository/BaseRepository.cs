using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Vendas.Repository
{
    public abstract class BaseRepository<T, K> : DbContext where T : class where K : BaseRepository<T, K>
    {
        public BaseRepository(DbContextOptions<K> options)
            : base(options)
        { }

        protected DbSet<T> Items { get; set; }

        public IEnumerable<T> ListAll()
        {
            return this.Items.ToArray();
        }

        public IQueryable<T> GetAll()
        {
            return this.Items.AsQueryable();
        }

        public void Add(T item)
        {
            this.Items.Add(item);
            this.SaveChanges();
        }

        public void Remove(T item)
        {
            this.Items.Remove(item);
        }

        public void Update(T item)
        {
            this.Items.Update(item);
            this.SaveChanges();
        }
    }
}
