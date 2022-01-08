using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    public class CoverTypeController : Controller
    {
        private readonly ICoverTypeRepository _coverTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(ICoverTypeRepository coverTypeRepository, IUnitOfWork unitOfWork)
        {
            _coverTypeRepository = coverTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var coverTypes = _coverTypeRepository.GetAll();
            return View(coverTypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType coverType)
        {
            if (!ModelState.IsValid)
            {
                return View(coverType);
            }

            _coverTypeRepository.Add(coverType);
            _unitOfWork.Commit();

            TempData["success"] = "Cover type created successfully";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var coverType = _coverTypeRepository.GetFirstOrDefault(ct => ct.Id == id);

            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType coverType)
        {
            if (!ModelState.IsValid)
            {
                return View(coverType);
            }

            _coverTypeRepository.Update(coverType);
            _unitOfWork.Commit();

            TempData["success"] = "Cover type updated successfully";

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var coverType = _coverTypeRepository.GetFirstOrDefault(ct => ct.Id == id);

            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            var coverType = _coverTypeRepository.GetFirstOrDefault(ct => ct.Id == id);

            if (coverType == null)
            {
                return NotFound();
            }

            _coverTypeRepository.Remove(coverType);
            _unitOfWork.Commit();

            TempData["success"] = "Cover type deleted successfully";

            return RedirectToAction("Index");
        }
    }
}
