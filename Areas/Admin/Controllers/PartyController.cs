using AccountingSuite.Data;
using AccountingSuite.Infrastructure;
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

        private void PopulateStatesDropdown()
        {
            var states = _stateRepository.GetAll() ?? new List<State>();
            ViewBag.States = new SelectList(states, "StateId", "StateName");
        }

        private void PopulatePartyTypes()
        {
            var types = Enum.GetValues(typeof(Party.PartyTypeEnum))
                            .Cast<Party.PartyTypeEnum>()
                            .Select(t => new SelectListItem { Text = t.ToString(), Value = t.ToString() })
                            .ToList();
            ViewBag.PartyTypes = types;
        }

        public IActionResult Index(string? searchTerm, int pageNumber = 1, int pageSize = 15)
        {
            var parties = _repository.GetAllWithState();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                parties = parties.Where(p =>
                    (!string.IsNullOrEmpty(p.PartyCode) && p.PartyCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
                ViewData["CurrentFilter"] = searchTerm;
            }
            
            PopulateStatesDropdown();
            var paginatedList = PaginatedList<Party>.Create(parties, pageNumber, pageSize);
            return View(paginatedList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateStatesDropdown();
            PopulatePartyTypes();
            return View(new Party { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Party party)
        {
            if (!ModelState.IsValid)
            {
                PopulateStatesDropdown();
                PopulatePartyTypes();
                return View(party);
            }

            try
            {
                var (newId, newCode) = _repository.Create(party);
                TempData["Message"] = $"Party created successfully with Code: {newCode}";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState);
                PopulateStatesDropdown();
                PopulatePartyTypes();
                return View(party);
            }
        }

        // GET: Party/Edit/5
        public IActionResult Edit(int id)
        {
            var party = _repository.GetById(id);
            if (party == null)
            {
                TempData["Error"] = "Party not found.";
                return RedirectToAction("Index");
            }

            PopulateStatesDropdown();
            PopulatePartyTypes();
            return View(party);
        }

        // POST: Party/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Party party)
        {
            if (!ModelState.IsValid)
            {
                PopulateStatesDropdown();
                PopulatePartyTypes();
                return View(party);
            }

            try
            {
                _repository.Update(party); // Update includes IsActive toggle
                TempData["Message"] = "Party updated successfully.";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                // Map SQL constraint errors to field-level messages
                SqlErrorMapper.Map(ex, ModelState);

                PopulateStatesDropdown();
                PopulatePartyTypes();
                return View(party);
            }
        }


        // GET: Party/Details/5
        public IActionResult Details(int id)
        {
            var party = _repository.GetById(id);
            if (party == null) return NotFound();

            return PartialView("_DetailsPartial", party); // 👈 return partial
        }

        // GET: Party/Delete/5
        public IActionResult Delete(int id)
        {
            var party = _repository.GetById(id);
            if (party == null) return NotFound();

            return PartialView("_DeletePartial", party); // 👈 return partial
        }

        // POST: Party/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int PartyId)
        {
            try
            {
                _repository.Delete(PartyId);
                TempData["Message"] = "Party deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState);
                TempData["Error"] = "Unable to delete Party due to database constraint.";
                return RedirectToAction("Index");
            }

        }
    }
}
