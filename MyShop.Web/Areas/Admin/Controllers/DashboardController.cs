using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Repositories;
using myshop.Utilities;

namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.AdminRole)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            ViewData["OrdersNumber"]=unitOfWork.OrderHeader.GetAll().Count();
            ViewData["ApprovedOrdersNumber"]=unitOfWork.OrderHeader.GetAll(e=>e.OrderStatus==SD.Approve).Count();
            ViewData["UsersNumber"]=unitOfWork.ApplicationUser.GetAll().Count();
            ViewData["ProductsNumber"]=unitOfWork.Product.GetAll().Count();
            
            return View();
        }
    }
}
