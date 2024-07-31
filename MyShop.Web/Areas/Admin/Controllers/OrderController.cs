using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using myshop.Utilities;
using Stripe;

namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetData()
        {
            IEnumerable<OrderHeader> OrderHeaders=
                unitOfWork.OrderHeader.GetAll(IncludeWord: "ApplicationUser").ToList();
            return Json(new {data= OrderHeaders });
        }

        public IActionResult Details(int orderid)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = unitOfWork.OrderHeader.GetFirstOrDefault(e => e.Id == orderid,IncludeWord: "ApplicationUser"),
                OrderDetails = unitOfWork.OrderDetail.GetAll(e => e.OrderHeaderId == orderid, IncludeWord: "Product")
            };

            return View(orderVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var orderFromDb = unitOfWork.OrderHeader.GetFirstOrDefault(e => e.Id == OrderVM.OrderHeader.Id);
           
            orderFromDb.Name = OrderVM.OrderHeader.Name;
            orderFromDb.Phone = OrderVM.OrderHeader.Phone;
            orderFromDb.Address = OrderVM.OrderHeader.Address;
            orderFromDb.City = OrderVM.OrderHeader.City;

            if (OrderVM.OrderHeader.Carrier != null)
            {
                orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }

			if (OrderVM.OrderHeader.TrackingNumber != null)
			{
				orderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber;
			}

            unitOfWork.OrderHeader.Update(orderFromDb);
            unitOfWork.Complete();

            TempData["Update"] = "Item has updated successfuly";
            return RedirectToAction("Details",new { orderid =orderFromDb.Id});

        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartProccess()
        {

            unitOfWork.OrderHeader.UpdateOrderStatus(OrderVM.OrderHeader.Id, SD.Proccessing, null);
            unitOfWork.Complete();


			TempData["Update"] = "Order Status has updated successfuly";
			return RedirectToAction("Details", new { orderid = OrderVM.OrderHeader.Id });
		}



		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShip()
		{
            var orderFromDb = unitOfWork.OrderHeader.GetFirstOrDefault(e => e.Id == OrderVM.OrderHeader.Id);

			orderFromDb.TrackingNumber=OrderVM.OrderHeader.TrackingNumber;
			orderFromDb.Carrier=OrderVM.OrderHeader.Carrier;
			orderFromDb.OrderStatus=SD.Shiped;
			orderFromDb.ShippingDate=DateTime.Now;
            unitOfWork.OrderHeader.Update(orderFromDb);
			unitOfWork.Complete();


			TempData["Update"] = "Order Status has Shipped successfuly";
			return RedirectToAction("Details", new { orderid = OrderVM.OrderHeader.Id });
		}





		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var orderFromDb = unitOfWork.OrderHeader.GetFirstOrDefault(e => e.Id == OrderVM.OrderHeader.Id);

            if(orderFromDb.PaymentStatus == SD.Approve)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromDb.PaymentIntendId
                };

                var service = new RefundService();
                Refund refund=service.Create(option);
                unitOfWork.OrderHeader.UpdateOrderStatus(OrderVM.OrderHeader.Id,SD.Cancelled,SD.Refund);
            }
            else
            {
                unitOfWork.OrderHeader.UpdateOrderStatus(OrderVM.OrderHeader.Id,SD.Cancelled,SD.Refund);
			}
            unitOfWork.Complete();

			TempData["Update"] = "Order has Cancelled Successfully";

			return RedirectToAction("Details", new { orderid = OrderVM.OrderHeader.Id });
		}






	}
}
