using myshop.Entities.Repositories;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
namespace myshop.DataAccess.Implementaion
{
	public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
	{
		private readonly AppDbContext context;

		public OrderDetailRepository(AppDbContext context) : base(context)
		{
			this.context = context;
		}

		public void Update(OrderDetail orderDetail)
		{
			context.OrderDetails.Update(orderDetail);
		}
	}
}
