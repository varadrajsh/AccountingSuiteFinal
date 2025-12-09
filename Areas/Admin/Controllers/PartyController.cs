using AccountingSuite.Data.Repositories;
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

        private async Task PopulateStatesDropdown()
        {
            var states = await _stateRepository.GetAllAsync();
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

        // Index with search + pagination
        public async Task<IActionResult> Index(string? searchTerm, int? stateId, int pageNumber = 1, int pageSize = 15)
        {
            var parties = await _repository.GetAllWithState();

            if (stateId.HasValue && stateId.Value > 0)
            {
                parties = parties.Where(p => p.StateId == stateId.Value).ToList();
                ViewData["SelectedState"] = stateId.Value;
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                parties = parties.Where(p =>
                    (!string.IsNullOrEmpty(p.PartyCode) && p.PartyCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
                ViewData["CurrentFilter"] = searchTerm;
            }

            await PopulateStatesDropdown();
            var paginatedList = PaginatedList<Party>.Create(parties, pageNumber, pageSize);
            return View(paginatedList);
        }

        // Show Create form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateStatesDropdown();
            PopulatePartyTypes();
            return View(new Party { IsActive = true });
        }

        // Handle Create submission
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

        // Show Edit form
        [HttpGet]
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

        // Handle Edit submission
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
                await _repository.Update(party);
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

        //   Details partial
        public async Task<IActionResult> Details(int id)
        {
            var party = await _repository.GetById(id);
            if (party == null) return NotFound();

            return PartialView("_DetailsPartial", party);
        }

        //   Delete partial
        public async Task<IActionResult> Delete(int id)
        {
            var party = await _repository.GetById(id);
            if (party == null) return NotFound();

            return PartialView("_DeletePartial", party);
        }

        //   Handle Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int PartyId)
        {
            try
            {
                await _repository.Delete(PartyId);
                TempData["Message"] = "Party deleted successfully.";
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState);
                TempData["Error"] = "Unable to delete Party due to database constraint.";
            }
            return RedirectToAction("Index");
        }
    }
}
