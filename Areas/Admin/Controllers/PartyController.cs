using AccountingSuite.Data.Repositories;
using AccountingSuite.Infrastructure;
using AccountingSuite.Models.Common;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using AccountingSuite.Data;
using AccountingSuite.Data.Repositories;

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

        private async Task PopulateStatesDropdown()
        {
            var states = await _stateRepository.GetAll();
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

        // ✅ Index with async repository calls
        public async Task<IActionResult> Index(string? searchTerm, int? stateId, int pageNumber = 1, int pageSize = 15)
        {
            var parties = await _repository.GetAllWithStateAsync();

            if (stateId.HasValue && stateId.Value > 0)
            {
                parties = parties.Where(p => p.StateId == stateId.Value);
                ViewData["SelectedState"] = stateId.Value;
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                parties = parties.Where(p =>
                    (!string.IsNullOrEmpty(p.PartyCode) && p.PartyCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
                ViewData["CurrentFilter"] = searchTerm;
            }

            await PopulateStatesDropdown();
            var paginatedList = PaginatedList<Party>.Create(parties, pageNumber, pageSize);
            return View(paginatedList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateStatesDropdown();
            PopulatePartyTypes();
            return View(new Party { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Party party)
        {
            if (!ModelState.IsValid)
            {
                await PopulateStatesDropdown();
                PopulatePartyTypes();
                return View(party);
            }

            try
            {
                var (newId, newCode) = await _repository.Create(party);
                TempData["Message"] = $"Party created successfully with Code: {newCode}";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState);
                await PopulateStatesDropdown();
                PopulatePartyTypes();
                return View(party);
            }
        }

        // GET: Party/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var party = await _repository.GetById(id);
            if (party == null)
            {
                TempData["Error"] = "Party not found.";
                return RedirectToAction("Index");
            }

            await PopulateStatesDropdown();
            PopulatePartyTypes();
            return View(party);
        }

        // POST: Party/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Party party)
        {
            if (!ModelState.IsValid)
            {
                await PopulateStatesDropdown();
                PopulatePartyTypes();
                return View(party);
            }

            try
            {
                await _repository.Update(party); // Update includes IsActive toggle
                TempData["Message"] = "Party updated successfully.";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState);
                await PopulateStatesDropdown();
                PopulatePartyTypes();
                return View(party);
            }
        }

        // GET: Party/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var party = await _repository.GetById(id);
            if (party == null) return NotFound();

            return PartialView("_DetailsPartial", party);
        }

        // GET: Party/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var party = await _repository.GetById(id);
            if (party == null) return NotFound();

            return PartialView("_DeletePartial", party);
        }

        // POST: Party/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int PartyId)
        {
            try
            {
                await _repository.Delete(PartyId);
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