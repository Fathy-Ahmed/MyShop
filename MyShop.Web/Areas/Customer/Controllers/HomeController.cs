using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Utilities;
using System.Security.Claims;
namespace MyShop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index(int? page)
        {
            return View(unitOfWork.Product.GetAll());
        }


        
        public IActionResult Detials(int ProductId)
        {
            ShoppingCart obj = new ShoppingCart()
            {
                productId=ProductId,
                Product = unitOfWork.Product.GetFirstOrDefault(e => e.Id == ProductId, IncludeWord: "Category"),
                Count = 1
            };
            return View(obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Detials(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart CartObj=unitOfWork.ShoppingCart.GetFirstOrDefault(e =>
            e.ApplicationUserId == claim.Value&& e.productId==shoppingCart.productId
            );
            if (CartObj == null)
            {
                unitOfWork.ShoppingCart.Add(shoppingCart);
                unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.ShoppingCartSession, 
                    unitOfWork.ShoppingCart.GetAll(e=>e.ApplicationUserId==claim.Value).ToList().Count()
                    );
               

            }
            else
            {
                unitOfWork.ShoppingCart.IncreaseCount(CartObj,shoppingCart.Count);
                unitOfWork.Complete();
            }


            return RedirectToAction("Index");
        }


    }
}
