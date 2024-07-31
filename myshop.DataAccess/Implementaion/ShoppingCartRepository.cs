using myshop.Entities.Repositories;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
namespace myshop.DataAccess.Implementaion
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly AppDbContext context;

        public ShoppingCartRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public int DecreaseCount(ShoppingCart shoppingCart, int count)
        {
            if(shoppingCart.Count-count >= 0)
            shoppingCart.Count-=count;
            return shoppingCart.Count;
        }

        public int IncreaseCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            var shoppingCartInDb = context.ShoppingCarts.FirstOrDefault(e => e.Id == shoppingCart.Id);
            if (shoppingCartInDb != null)
            {
                shoppingCartInDb.ApplicationUserId = shoppingCart.ApplicationUserId;
                shoppingCartInDb.productId = shoppingCart.productId;
                shoppingCartInDb.productId = shoppingCart.productId;

            }
        }
    }
}
