

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Repositories;
using myshop.Utilities;
using System.Security.Claims;

namespace myshop.Entities.ViewComponents
{
    public class ShoppingCartViewComponent:ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.ShoppingCartSession) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.ShoppingCartSession));
                }
                else
                {
                    HttpContext.Session.SetInt32(SD.ShoppingCartSession,
                        unitOfWork.ShoppingCart.GetAll(e=>e.ApplicationUserId== claim.Value).ToList().Count()
                        );
                    return View(HttpContext.Session.GetInt32(SD.ShoppingCartSession));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }

    }
}
