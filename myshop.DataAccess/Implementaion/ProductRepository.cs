using myshop.Entities.Repositories;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
namespace myshop.DataAccess.Implementaion
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext context;

        public ProductRepository(AppDbContext context):base(context)    
        {
            this.context = context;
        }
        public void Update(Product product)
        {
            var productInDb=context.Products.FirstOrDefault(e=> e.Id == product.Id);
            if (productInDb != null)
            {
                productInDb.Name = product.Name;
                productInDb.Description = product.Description;
                productInDb.Price = product.Price;
                productInDb.Img = product.Img;
                productInDb.CategoryId = product.CategoryId;
                //productInDb.Category= product.Category;

            }
        }

       
    }
}
