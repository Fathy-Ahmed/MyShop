using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Data;
using myshop.Utilities;
using System.Security.Claims;

namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly AppDbContext context;

        public UsersController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim= claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            return View(context.ApplicationUsers.Where(e=>e.Id!=userId).ToList());
        }

        public IActionResult LockUnLock(string id)
        {
            var user=context.ApplicationUsers.FirstOrDefault(e=>e.Id==id);
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
            context.SaveChanges();
            return RedirectToAction("Index","Users",new {area="admin"});
        }
    }
}
