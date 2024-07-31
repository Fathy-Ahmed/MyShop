using myshop.Entities.Repositories;
using myshop.DataAccess.Data;
using MyShop.Entities.Models;
namespace myshop.DataAccess.Implementaion
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext context;

        public CategoryRepository(AppDbContext context):base(context) 
        {
            this.context = context;
        }
        public void Update(Category category)
        {
            var CategoryInDb=context.Categories.FirstOrDefault(e=> e.Id == category.Id);
            if (CategoryInDb != null)
            {
                CategoryInDb.Name = category.Name;
                CategoryInDb.Description = category.Description;
                CategoryInDb.CreatedTime = DateTime.Now;
            }
        }
    }





}
