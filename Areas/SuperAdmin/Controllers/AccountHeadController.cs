using AccountingSuite.Data;
using AccountingSuite.Infrastructure;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AccountingSuite.Models.Common;


namespace AccountingSuite.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class AccountHeadController : Controller
    {
        private readonly AccountHeadRepository _repo;
        public AccountHeadController(AccountHeadRepository repo)
        {
            _repo = repo;
        }


        // GET: AccountHead
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var accountHeads = await _repo.GetAllAsync();
            var paginated = PaginatedList<AccountHead>.Create(accountHeads, pageNumber, 10);
            return View(paginated);
        }

        // Get Details
        public async Task<IActionResult> Details(int id)
        {
            var accountHead = await _repo.GetByIdAsync(id);
            if (accountHead == null) return NotFound();
            return View(accountHead);
        }

        //Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountHead accountHead)
        {
            try
            {
                var result = await _repo.CreateAsync(accountHead, ModelState);

                if (ModelState.IsValid && result.newId > 0)
                {
                    TempData["Message"] = $"Account Head '{accountHead.AccountHeadName}' created. Code: {result.newCode}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unexpected error occurred while creating Account Head.");
            }

            return View(accountHead);
        }

        //Get Edit
        public async Task<IActionResult> EditStatus(int id)
        {
            var accountHead = await _repo.GetByIdAsync(id);
            if (accountHead == null) return NotFound();

            return View(accountHead);
        }

        //Post Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(int id, bool isActive)
        {
            try
            {
                var success = await _repo.UpdateStatusAsync(id, isActive, ModelState);
                if (!success || !ModelState.IsValid)
                {
                    var accountHead = await _repo.GetByIdAsync(id);
                    return View(accountHead);
                }
                TempData["Message"] = "Account Head status updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState);
                var accountHead = await _repo.GetByIdAsync(id);
                return View(accountHead);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unexpected error occurred while creating Account Head.");
                var accountHead = await _repo.GetByIdAsync(id);
                return View(accountHead);
            }
        }
    }
}
