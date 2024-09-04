using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using myshop.DataAccess.Implementaion;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using myshop.Utilities;
using Stripe.Checkout;
using System.Security.Claims;

namespace MyShop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            shoppingCartVM = new ShoppingCartVM()
            {
                CrartsList = unitOfWork.ShoppingCart.GetAll(e => e.ApplicationUserId == userId,IncludeWord:"Product"),
				OrderHeader=new()
            };
            foreach (var item in shoppingCartVM.CrartsList)
            {
                shoppingCartVM.OrderHeader.OrderTotalPrice += (item.Product.Price*item.Count);

			}
            return View(shoppingCartVM);
        }

        [HttpGet]
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			string userId = claim.Value;

            shoppingCartVM = new ShoppingCartVM()
            {
                CrartsList = unitOfWork.ShoppingCart.GetAll(e => e.ApplicationUserId == userId, IncludeWord: "Product"),
                OrderHeader = new()
            };

            shoppingCartVM.OrderHeader.ApplicationUser = unitOfWork.ApplicationUser.GetFirstOrDefault(
                e=>e.Id==userId
                );

			shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
			shoppingCartVM.OrderHeader.Address = shoppingCartVM.OrderHeader.ApplicationUser.Address;
			shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
			shoppingCartVM.OrderHeader.Phone = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            foreach (var item in shoppingCartVM.CrartsList)
            {
                shoppingCartVM.OrderHeader.OrderTotalPrice += (item.Product.Price * item.Count);

            }

            return View(shoppingCartVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Summary(ShoppingCartVM ShoppingCartVM)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			string userId = claim.Value;

            ShoppingCartVM.CrartsList = unitOfWork.ShoppingCart.GetAll(e => e.ApplicationUserId == userId, IncludeWord: "Product");

			ShoppingCartVM.OrderHeader.OrderStatus = SD.Pending;
			ShoppingCartVM.OrderHeader.PaymentStatus = SD.Pending;
			ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			foreach (var item in ShoppingCartVM.CrartsList)
			{
				ShoppingCartVM.OrderHeader.OrderTotalPrice += (item.Count * item.Product.Price);
			}

			unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			unitOfWork.Complete();

			foreach (var item in ShoppingCartVM.CrartsList)
			{
				OrderDetail orderDetail = new OrderDetail()
				{
					ProductId = item.productId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = item.Product.Price,
					Count = item.Count
				};

				unitOfWork.OrderDetail.Add(orderDetail);
				unitOfWork.Complete();
			}

			//////////// stripe

			var domain = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json").Build().GetSection("domain").Value;


			var options = new SessionCreateOptions
			{
				LineItems = new List<SessionLineItemOptions>(),

				Mode = "payment",
				SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
				CancelUrl = domain + $"customer/cart/index",
			};

			foreach (var item in ShoppingCartVM.CrartsList)
			{
				var sessionlineoption = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Product.Price * 100),
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name,
						},
					},
					Quantity = item.Count,
				};
				options.LineItems.Add(sessionlineoption);
			}


			var service = new SessionService();
			Session session = service.Create(options);
			ShoppingCartVM.OrderHeader.SessionId = session.Id;

            unitOfWork.Complete();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}

		public IActionResult OrderConfirmation(int id)
		{
			var orderHeader=unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

			if (session.PaymentStatus.ToLower() == "paid")
			{
				unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.Approve, SD.Approve);
                orderHeader.PaymentIntendId = session.PaymentIntentId;
				unitOfWork.Complete();
            }

			var ShoppingCarts = 
                unitOfWork.ShoppingCart.GetAll(e=>e.ApplicationUserId==orderHeader.ApplicationUserId).ToList();
            HttpContext.Session.Clear();
            unitOfWork.ShoppingCart.RemoveRange(ShoppingCarts);
            unitOfWork.Complete();

            return View(id);
		}


        public IActionResult Plus(int cartid)
        {
            var cart=unitOfWork.ShoppingCart.GetFirstOrDefault(e=>e.Id==cartid);
            unitOfWork.ShoppingCart.IncreaseCount(cart, 1);
            unitOfWork.Complete();
			return RedirectToAction("Index");

		}
		public IActionResult Minus(int cartid)
		{
			var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(e => e.Id == cartid);
			unitOfWork.ShoppingCart.DecreaseCount(cart, 1);
			if (cart.Count == 0)
			{
				unitOfWork.ShoppingCart.Remove(cart);
                unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.ShoppingCartSession,
                 unitOfWork.ShoppingCart.GetAll(e => e.ApplicationUserId == cart.ApplicationUserId).ToList().Count()
                 );
            }
            unitOfWork.Complete();
            
			return RedirectToAction("Index");
		}
		public IActionResult Remove(int cartid)
        {
			var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(e => e.Id == cartid);
            if(cart!=null)
			unitOfWork.ShoppingCart.Remove(cart);
            unitOfWork.Complete();
            HttpContext.Session.SetInt32(SD.ShoppingCartSession,
                 unitOfWork.ShoppingCart.GetAll(e => e.ApplicationUserId == cart.ApplicationUserId).ToList().Count()
                 );
            return RedirectToAction("Index");

		}

	}
}
