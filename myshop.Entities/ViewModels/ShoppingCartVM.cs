
using myshop.Entities.Models;

namespace myshop.Entities.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> CrartsList { get; set; }
        public OrderHeader OrderHeader { get; set; }

    }
}
