using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Repositories;
using MyShop.Entities.Models;

namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View(unitOfWork.Category.GetAll());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Add(category);
                unitOfWork.Complete();
                TempData["Create"] = "Category Has Created Successfuly";
                return RedirectToAction("Index");
            }

            return View(category);
        }





        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(unitOfWork.Category.GetFirstOrDefault(e => e.Id == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Update(category);
                unitOfWork.Complete();

                TempData["Update"] = "Category Has Updated Successfuly";
                return RedirectToAction("Index");
            }

            return View(category);
        }



        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(unitOfWork.Category.GetFirstOrDefault(e => e.Id == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalDelete(int id)
        {
            var category = unitOfWork.Category.GetFirstOrDefault(e => e.Id == id);
            if (category == null)
            {
                NotFound();
            }
            unitOfWork.Category.Remove(category);
            unitOfWork.Complete();
            TempData["Delete"] = "Category Has Deleted Successfuly";
            return RedirectToAction("Index");
        }


    }
}
