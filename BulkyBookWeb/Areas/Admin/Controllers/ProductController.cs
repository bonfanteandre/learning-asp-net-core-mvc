using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICoverTypeRepository _coverTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ICoverTypeRepository coverTypeRepository,
            IUnitOfWork unitOfWork, 
            IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _coverTypeRepository = coverTypeRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _productRepository.GetAll(new string[] { "Category", "CoverType" });
            return Json(new
            {
                Data = products
            });
        }

        public IActionResult Upsert(int? id)
        {
            var productViewModel = new ProductViewModel();
            productViewModel.Product = new Product();

            productViewModel.Categories = _categoryRepository
                .GetAll()
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

            productViewModel.CoverTypes = _coverTypeRepository
                .GetAll()
                .Select(ct => new SelectListItem
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString()
                });

            if (id != null)
            {
                productViewModel.Product = _productRepository.GetFirstOrDefault(p => p.Id == id);
            }

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productViewModel, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                productViewModel.Categories = _categoryRepository
                .GetAll()
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

                productViewModel.CoverTypes = _coverTypeRepository
                    .GetAll()
                    .Select(ct => new SelectListItem
                    {
                        Text = ct.Name,
                        Value = ct.Id.ToString()
                    });

                return View(productViewModel);
            }

            if (file != null)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                string fileExtension = Path.GetExtension(file.FileName);
                string filename = $"{Guid.NewGuid()}{fileExtension}";
                string path = Path.Combine(rootPath, @"images\products", filename);

                if (!string.IsNullOrWhiteSpace(productViewModel.Product.ImageUrl))
                {
                    var oldPath = Path.Combine(rootPath, @"images\products", productViewModel.Product.ImageUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productViewModel.Product.ImageUrl = filename;
            }

            string successMessage = string.Empty;

            if (productViewModel.Product.Id == 0)
            {
                _productRepository.Add(productViewModel.Product);
                successMessage = "Product created successfully";
            }
            else
            {
                _productRepository.Update(productViewModel.Product);
                successMessage = "Product updated successfully";
            }

            _unitOfWork.Commit();

            TempData["success"] = successMessage;

            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetFirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Unable to find product"
                });
            }

            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                var oldPath = Path.Combine(rootPath, @"images\products", product.ImageUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            _productRepository.Remove(product);
            _unitOfWork.Commit();

            return Json(new
            {
                Success = true,
                Message = "Product successfully deleted"
            });
        }
    }
}
