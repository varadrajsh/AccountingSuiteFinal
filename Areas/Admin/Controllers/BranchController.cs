using AccountingSuite.Data.Repositories;
using AccountingSuite.Models.Common;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AccountingSuite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BranchController : Controller
    {
        private readonly BranchRepository _repo;
        private readonly StateRepository _stateRepo;

        public BranchController(BranchRepository repo, StateRepository stateRepo)
        {
            _repo = repo;
            _stateRepo = stateRepo;
        }

        private async Task PopulateStatesDropdown(int? selectedStateId = null)
        {
            var states = await _stateRepo.GetAllAsync();
            ViewBag.States = new SelectList(states, "StateId", "StateName", selectedStateId);
        }

        public async Task<IActionResult> Index(int? stateId, string searchTerm, int pageNumber = 1)
        {
            await PopulateStatesDropdown(stateId);
            var branches = await _repo.GetAllWithStateAsync();

            if (stateId.HasValue && stateId.Value > 0)
            {
                branches = branches.Where(b => b.StateId == stateId.Value).ToList();
                ViewData["SelectedState"] = stateId.Value;
            }

            if (!string.IsNullOrEmpty(searchTerm))
                branches = branches.Where(b =>
                 (!string.IsNullOrEmpty(b.BranchName) && b.BranchName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(b.BranchCode) && b.BranchCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(b.Email) && b.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(b.Address) && b.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(b.PinCode) && b.PinCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(b.LandNumber) && b.LandNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(b.MobNumber) && b.MobNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        ).ToList();

            var paginated = PaginatedList<Branch>.Create(branches.ToList(), pageNumber, 15);
            return View(paginated);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            await PopulateStatesDropdown();
            return View(new Branch { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                // repopulate dropdowns
                ViewBag.States = new SelectList(await _stateRepo.GetAllAsync(), "StateId", "StateName");
                return View(branch);
            }

            var result = await _repo.CreateAsync(branch, ModelState);
            if (result == 1)
            {
                TempData["Message"] = "Branch created successfully.";
                return RedirectToAction(nameof(Index));
            }
            // If repository added errors, show them
            if (!ModelState.IsValid)
            {
                await PopulateStatesDropdown(branch.StateId);
                return View(branch);
            }

            TempData["Error"] = "Failed to create branch.";
            ViewBag.States = new SelectList(await _stateRepo.GetAllAsync(), "StateId", "StateName");
            return View(branch);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _repo.GetByIdAsync(id);
            if (branch == null) return NotFound();

            await PopulateStatesDropdown(branch.StateId);
            return View(branch);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Branch branch)
        {
            var result = await _repo.UpdateAsync(branch, ModelState);
            if (result > 0)
            {
                TempData["Message"] = "Branch updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            // If update failed
            if (!ModelState.IsValid)
            {
                await PopulateStatesDropdown(branch.StateId);
                return View(branch);
            }

            TempData["Error"] = "Failed to update branch.";
            await PopulateStatesDropdown(branch.StateId);
            return View(branch);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int branchId, bool isActive)
        {
            await _repo.UpdateStatusAsync(branchId, isActive, ModelState);

            if (ModelState.ErrorCount == 0)
                TempData["Message"] = "Branch status updated.";
            else
                TempData["Error"] = "Unable to update branch status.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var branch = await _repo.GetByIdAsync(id);
            if (branch == null) return NotFound();
            return PartialView("_BranchDetails", branch);
        }

           // Action to return partial branch table by state
        public async Task<IActionResult> TableByState(int stateId, int pageNumber = 1)
        {
            // Fetch branches for the given state with pagination
            var branches = await _repo.GetBranchesByStateAsync(stateId, pageNumber);

            // Return the partial view (your branch table snippet)
            return PartialView("_BranchTablePartial", branches);
        }
    }


}
