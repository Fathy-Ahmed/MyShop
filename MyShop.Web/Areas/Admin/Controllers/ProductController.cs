using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;

namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {

            return View(new { data = unitOfWork.Product.GetAll() });
        }

        public IActionResult GetData()
        {
            return Json(unitOfWork.Product.GetAll(IncludeWord: "Category"));
        }


        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = unitOfWork.Category.GetAll().Select(x =>
                    new SelectListItem { Text = x.Name, Value = x.Id.ToString() }
                )
            };
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {

                string RootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Products");
                    var extension = Path.GetExtension(file.FileName);

                    using (var fileStream = new FileStream(Path.Combine(Upload, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.Img = @"Images\Products\" + fileName + extension;

                }

                unitOfWork.Product.Add(productVM.Product);
                unitOfWork.Complete();
                TempData["Create"] = "Product Has Created Successfuly";
                return RedirectToAction("Index");
            }

            productVM.CategoryList = unitOfWork.Category.GetAll().Select(x =>
                    new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            return View(productVM);
        }




        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            ProductVM productVM = new ProductVM()
            {
                Product = unitOfWork.Product.GetFirstOrDefault(e => e.Id == id),
                CategoryList = unitOfWork.Category.GetAll().Select(x =>
                    new SelectListItem { Text = x.Name, Value = x.Id.ToString() }
                )
            };
            return View(productVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string RootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Products");
                    var extension = Path.GetExtension(file.FileName);

                    if (productVM.Product.Img != null)
                    {
                        var oldImg = Path.Combine(RootPath, productVM.Product.Img.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImg))
                        {
                            System.IO.File.Delete(oldImg);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(Upload, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.Img = @"Images\Products\" + fileName + extension;

                }


                unitOfWork.Product.Update(productVM.Product);
                unitOfWork.Complete();

                TempData["Update"] = "Product Has Updated Successfuly";
                return RedirectToAction("Index");
            }

            productVM.CategoryList = unitOfWork.Category.GetAll().Select(x =>
                   new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            return View(productVM);
        }




        [HttpDelete]
        // [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var Product = unitOfWork.Product.GetFirstOrDefault(e => e.Id == id);
            if (Product == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            unitOfWork.Product.Remove(Product);
            ////////////    Delete the img ////////////////
            var oldImg = Path.Combine(webHostEnvironment.WebRootPath, Product.Img.TrimStart('\\'));
            if (System.IO.File.Exists(oldImg))
            {
                System.IO.File.Delete(oldImg);
            }
            ////////////////////////////////////////////////
            unitOfWork.Complete();
            TempData["Delete"] = "Product Has Deleted Successfuly";
            return Json(new { success = true, message = "file has been Deleted" });

        }


    }
}
