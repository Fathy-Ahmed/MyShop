
using Microsoft.EntityFrameworkCore;
using myshop.Entities.Repositories;
using myshop.DataAccess.Data;
using System.Linq.Expressions;

namespace myshop.DataAccess.Implementaion
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext context;
        private DbSet<T> dbSet;
        public GenericRepository(AppDbContext context)
        {
            this.context = context;
            dbSet=context.Set<T>();
        }

        public void Add(T entity)
        {
           
            dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> Predicate, string? IncludeWord)
        {
            IQueryable<T> query = dbSet;
            if(Predicate != null)
            {
                query= query.Where(Predicate);
            }
            if(IncludeWord != null)
            {
                // context.products.Include("Category,users,logs")
                foreach(var item in IncludeWord.Split(',',StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(item);
                }
            }
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> Predicate, string? IncludeWord)
        {
            IQueryable<T> query = dbSet;
            if (Predicate != null)
            {
                query = query.Where(Predicate);
            }
            if (IncludeWord != null)
            {
                // context.products.Include("Category,users,logs")
                foreach (var item in IncludeWord.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.SingleOrDefault();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
