using myshop.Entities.Repositories;
using myshop.DataAccess.Data;

namespace myshop.DataAccess.Implementaion
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }


        public UnitOfWork(AppDbContext context)
        {
            this.context = context;
            Category =new CategoryRepository(context);
            Product =new ProductRepository(context);
            ShoppingCart=new ShoppingCartRepository(context);
			OrderDetail =new OrderDetailRepository(context);
            OrderHeader =new OrderHeaderRepository(context);
            ApplicationUser =new ApplicationUserRepository(context);

		}

        public int Complete()
        {
            return context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
