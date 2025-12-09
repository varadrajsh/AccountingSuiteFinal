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


        //Index action uses lightweight list
        public async Task<IActionResult> Index(int? stateId, string searchTerm, int pageNumber = 1)
        {
            await PopulateStatesDropdown(stateId);

            var branches = await _repo.GetAllWithState();


            if (stateId.HasValue && stateId.Value > 0)
            {
                branches = branches.Where(b => b.StateId == stateId.Value).ToList();
                ViewData["SelectedState"] = stateId.Value; // store selected state
            }


            if (!string.IsNullOrEmpty(searchTerm))
                branches = branches.Where(b => b.BranchName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            var paginated = PaginatedList<Branch>.Create(branches, pageNumber, 15); // Alter Number of Results to Display in Page 
            return View(paginated);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateStatesDropdown();
            return View(new Branch { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                await PopulateStatesDropdown();
                return View(branch);
            }

            try
            {
                branch.BranchCode = branch.BranchCode.Trim().ToUpperInvariant();
                branch.BranchName = branch.BranchName.Trim().ToUpperInvariant();

                await _repo.Insert(branch);
                TempData["Message"] = "Branch created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            await PopulateStatesDropdown();
            return View(branch);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _repo.GetById(id);
            if (branch == null) return NotFound();

            await PopulateStatesDropdown();
            ViewBag.States = new SelectList(await _stateRepo.GetAllAsync(), "StateId", "StateName");
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                await PopulateStatesDropdown();
                return View(branch);
            }

            try
            {
                branch.BranchCode = branch.BranchCode.Trim().ToUpperInvariant();
                branch.BranchName = branch.BranchName.Trim().ToUpperInvariant();

                await _repo.Update(branch);
                TempData["Message"] = "Branch updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            await PopulateStatesDropdown();
            return View(branch);
        }

        public async Task<IActionResult> Details(int id)
        {
            var branch = await _repo.GetById(id);
            if (branch == null) return NotFound();

            return PartialView("_DetailsPartial", branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int branchId, bool isActive)
        {
            try
            {
                await _repo.UpdateStatus(branchId, isActive);
                TempData["Message"] = "Branch status updated.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _repo.Delete(id);
                TempData["Message"] = "Branch deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unable to delete branch: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
