using myshop.Entities.Models;

namespace myshop.Entities.Repositories
{
	public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
	{
		void Update(OrderHeader orderHeader);
		void UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus);
	}

	public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
	{
		void Update(OrderDetail orderDetail);
	}

}
