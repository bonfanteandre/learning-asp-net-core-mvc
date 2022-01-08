using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = _companyRepository.GetAll();
            return Json(new
            {
                Data = companies
            });
        }

        public IActionResult Upsert(int? id)
        {
            var company = new Company();

            if (id != null)
            {
                company = _companyRepository.GetFirstOrDefault(p => p.Id == id);
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (!ModelState.IsValid)
            {
                return View(company);
            }

            string successMessage = string.Empty;

            if (company.Id == 0)
            {
                _companyRepository.Add(company);
                successMessage = "Company created successfully";
            }
            else
            {
                _companyRepository.Update(company);
                successMessage = "Company updated successfully";
            }

            _unitOfWork.Commit();

            TempData["success"] = successMessage;

            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var company = _companyRepository.GetFirstOrDefault(p => p.Id == id);

            if (company == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Unable to find company"
                });
            }

            _companyRepository.Remove(company);
            _unitOfWork.Commit();

            return Json(new
            {
                Success = true,
                Message = "Company successfully deleted"
            });
        }
    }
}
