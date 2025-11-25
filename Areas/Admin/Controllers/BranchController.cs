using AccountingSuite.Data;
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

        public IActionResult Index(int? stateId, string searchTerm, int pageNumber = 1)
        {
            // ✅ Populate dropdown
            ViewBag.States = _stateRepo.GetAll();

            var branches = _repo.GetAll();

            if (stateId.HasValue)
                branches = branches.Where(b => b.StateId == stateId.Value);

            if (!string.IsNullOrEmpty(searchTerm))
                branches = branches.Where(b => b.BranchName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            var paginated = PaginatedList<Branch>.Create(branches, pageNumber, 10);
            return View(paginated);
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.States = _stateRepo.GetAll()
                .Select(s => new SelectListItem { Value = s.StateId.ToString(), Text = s.StateName })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Branch branch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    branch.BranchCode = branch.BranchCode.Trim().ToUpperInvariant();
                    branch.BranchName = branch.BranchName.Trim().ToUpperInvariant();

                    _repo.Insert(branch); // ✅ no need to set IsActive, SP enforces it
                    TempData["Message"] = "Branch created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            ViewBag.States = _stateRepo.GetAll()
                .Select(s => new SelectListItem { Value = s.StateId.ToString(), Text = s.StateName })
                .ToList();

            return View(branch);
        }



        [HttpPost]
        public IActionResult UpdateStatus(int branchId, bool isActive)
        {
            try
            {
                _repo.UpdateStatus(branchId, isActive);
                TempData["Message"] = "Branch status updated.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var branch = _repo.GetById(id);
            if (branch == null) return NotFound();

            ViewBag.States = _stateRepo.GetAll()
                .Select(s => new SelectListItem { Value = s.StateId.ToString(), Text = s.StateName })
                .ToList();

            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Branch branch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    branch.BranchCode = branch.BranchCode.Trim().ToUpperInvariant();
                    branch.BranchName = branch.BranchName.Trim().ToUpperInvariant();

                    _repo.Update(branch);
                    TempData["Message"] = "Branch updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            ViewBag.States = _stateRepo.GetAll()
                .Select(s => new SelectListItem { Value = s.StateId.ToString(), Text = s.StateName })
                .ToList();

            return View(branch);
        }

        public IActionResult Details(int id)
        {
            var branch = _repo.GetById(id);
            if (branch == null)
            {
                return NotFound();
            }
            return PartialView("_DetailsPartial", branch);
        }

        public IActionResult TableByState(int stateId, int pageNumber = 1, int pageSize = 10)
        {
            var branches = _repo.GetAll().Where(b => b.StateId == stateId);

            var paginated = PaginatedList<Branch>.Create(branches, pageNumber, pageSize);

            // Partial must exist in Areas/Admin/Views/Branch/_BranchTablePartial.cshtml
            return PartialView("_BranchTablePartial", paginated);
        }


    }
}
