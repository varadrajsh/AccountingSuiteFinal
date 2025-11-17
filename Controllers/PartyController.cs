using AccountingSuite.Data;
using AccountingSuite.Models.Common;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSuite.Controllers
{
    public class PartyController : Controller
    {
        private readonly PartyRepository _repository;
        public PartyController(PartyRepository partyRepository)
        {
            _repository = partyRepository;
        }

        public IActionResult Index(string? searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var parties = _repository.GetAll();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                parties = parties.Where(p =>
                    p.PartyCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
                ViewData["CurrentFilter"] = searchTerm;
            }

            var paginatedList = PaginatedList<Party>.Create(parties, pageNumber, pageSize);
            return View(paginatedList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Party party)
        {
            if (!ModelState.IsValid) return View(party);

            try
            {
                var result = _repository.Create(party);
                TempData["Message"] = $"Party created successfully with Code: {result.newCode}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", ex.Message);
                TempData["Error"] = ex.Message; 
                return View(party);
            }
        }

        public IActionResult Details(int id)
        {
            var party = _repository.GetById(id);
            if (party == null) return NotFound();
            return PartialView("_Details", party);
        }

        public IActionResult Edit(int id)
        {
            var party = _repository.GetById(id);
            if (party == null) return NotFound();
            return PartialView("_Edit", party);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Party party)
        {
            if (!ModelState.IsValid) return PartialView("_Edit", party);

            try
            {
                var result = _repository.Update(party);
                TempData["Message"] = "Party Updated successfully.";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                //ModelState.AddModelError("", ex.Message);
                return PartialView("_Edit", party);
            }
        }
        public IActionResult Delete(int id)
        {
            var party = _repository.GetById(id);
            if (party == null) return NotFound();
            return PartialView("_Delete", party);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int PartyId)
        {
            try
            {
                _repository.Delete(PartyId);
                TempData["Message"] = "Party deleted successfully.";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // ✅ store error in TempData
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
