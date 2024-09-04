using myshop.Entities.Repositories;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
namespace myshop.DataAccess.Implementaion
{
	public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly AppDbContext context;

		public OrderHeaderRepository(AppDbContext context) : base(context)
		{
			this.context = context;
		}

		public void Update(OrderHeader orderHeader)
		{
			context.OrderHeaders.Update(orderHeader);
		}

		public void UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus)
		{
			var OrderHeaderInDb = context.OrderHeaders.FirstOrDefault(e => e.Id == id);
            if (OrderHeaderInDb != null)
            {
				OrderHeaderInDb.OrderStatus = OrderStatus;
				OrderHeaderInDb.PaymentDate = DateTime.Now;
				if (PaymentStatus != null)
				OrderHeaderInDb.PaymentStatus = PaymentStatus;

			}
		}
		

	}
}
