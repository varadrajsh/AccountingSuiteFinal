using AccountingSuite.Data;
using AccountingSuite.Data.Repositories;
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

        public IActionResult Index(int? stateId)
        {
            IEnumerable<Branch> branches = stateId.HasValue
                ? _repo.GetAll().Where(b => b.StateId == stateId.Value)
                : _repo.GetAll();

            ViewBag.States = _stateRepo.GetAll()
                .Select(s => new SelectListItem { Value = s.StateId.ToString(), Text = s.StateName })
                .ToList();

            return View(branches);
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
                    _repo.Insert(branch);
                    TempData["Message"] = "Branch created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
          

            // reload states for redisplay
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
    }
}
