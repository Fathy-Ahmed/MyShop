using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Data;
using myshop.Entities.Repositories;
using myshop.Utilities;
using System.Security.Claims;

namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim= claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            //return View(context.ApplicationUsers.Where(e=>e.Id!=userId).ToList());
            return View(unitOfWork.ApplicationUser.GetAll(e=>e.Id!=userId));
        }

        public IActionResult LockUnLock(string id)
        {
           // var user=context.ApplicationUsers.FirstOrDefault(e=>e.Id==id);
            var user= unitOfWork.ApplicationUser.GetFirstOrDefault(e => e.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(10);
            }
            else
            {
                user.LockoutEnd = null;
            }
            unitOfWork.Complete();
            return RedirectToAction("Index","Users",new {area="admin"});
        }

       
        public IActionResult Delete(string id)
        {
            var user=unitOfWork.ApplicationUser.GetFirstOrDefault(c=>c.Id==id);
            if (user != null)
            {
                unitOfWork.ApplicationUser.Remove(user);
                unitOfWork.Complete();
            }
            return RedirectToAction("Index", "Users", new { area = "admin" });
        }
    }
}
