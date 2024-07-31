

using System.Linq.Expressions;

namespace myshop.Entities.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // context.Categories.Include(e=>e.Products).ToList();
        // context.Categories.Where(e=>e.id == id).ToList();
        IEnumerable<T> GetAll(Expression<Func<T,bool>>? Predicate = null,string? IncludeWord=null);

        // context.Categories.Include(e=>e.Products).ToList();
        // context.Categories.Where(e=>e.id == id).ToList();

        T GetFirstOrDefault(Expression<Func<T, bool>>? Predicate=null, string? IncludeWord = null);

        // context.Categories.Add(category)
        void Add(T entity);

        // context.Categories.Remove(category)
        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

    }
}
