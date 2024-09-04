

using myshop.Entities.Models;

namespace myshop.Entities.Repositories
{
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        void Update(ShoppingCart shoppingCart);

        int IncreaseCount(ShoppingCart shoppingCart,int count);
        int DecreaseCount(ShoppingCart shoppingCart,int count);
    }
}
