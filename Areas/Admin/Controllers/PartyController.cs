using AccountingSuite.Data;
using AccountingSuite.Models.Common;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;


namespace AccountingSuite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PartyController : Controller
    {
        private readonly PartyRepository _repository;
        private readonly StateRepository _stateRepository;
        public PartyController(PartyRepository partyRepository, StateRepository stateRepository)
        {
            _repository = partyRepository;
            _stateRepository = stateRepository;
        }

        public IActionResult Index(string? searchTerm, int pageNumber = 1, int pageSize = 15)
        {
            // var parties = _repository.GetAll();
            var parties = _repository.GetAllWithState();
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
            var states = _stateRepository.GetAll() ?? new List<State>();
            ViewBag.States = new SelectList(states, "StateId", "StateName");
            return View(new Party());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Party party)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.States = new SelectList(_stateRepository.GetAll(), "StateId", "StateName");
                return View(party);
            }

            try
            {
                var (newId, newCode) = _repository.Create(party);
                TempData["Message"] = $"Party created successfully with Code: {newCode}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Show on the same page
                ModelState.AddModelError("", ex.Message);
                ViewBag.States = new SelectList(_stateRepository.GetAll(), "StateId", "StateName");
                return View(party);
            }
        }



        [HttpGet]
        public IActionResult Edit(int id)
        {
            var party = _repository.GetById(id);
            if (party == null) return NotFound();

            var state = _stateRepository.GetById(party.StateId);
            ViewBag.StateName = state?.StateName ?? "";

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

                //TempData["Error"] = ex.Message;
                ModelState.AddModelError("", ex.Message);
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
