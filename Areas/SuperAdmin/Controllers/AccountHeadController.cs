using AccountingSuite.Data;
using AccountingSuite.Models.Master;
using AccountingSuite.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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

        // Index with pagination
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 25)
        {
            var accountHeads = await _repo.GetAllAsync();
            var paginated = PaginatedList<AccountHead>.Create(accountHeads, pageNumber, pageSize);
            return View(paginated);
        }

        // Create AccountHead via AccountLookup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountHead accountHead, int pageNumber = 1)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data. Please check the form and try again.";
                var accountHeads = await _repo.GetAllAsync();
                var paginated = PaginatedList<AccountHead>.Create(accountHeads, pageNumber, 25);
                return View("Index", paginated);
            }

            var result = await _repo.CreateAsync(accountHead, ModelState);

            if (result.newId > 0)
            {
                TempData["Message"] = $"Account Head '{accountHead.AccountHeadName}' created successfully.";
                return RedirectToAction(nameof(Index), new { pageNumber });
            }

            // If errors mapped by SqlErrorMapper
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var accountHeadsReload = await _repo.GetAllAsync();
            var paginatedReload = PaginatedList<AccountHead>.Create(accountHeadsReload, pageNumber, 25);
            return View("Index", paginatedReload);
        }

        // Autocomplete for AccountLookup
        [HttpGet]
        public async Task<JsonResult> SearchAccountLookup(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(Enumerable.Empty<object>());
            }

            var results = await _repo.SearchAccountLookupAsync(term);

            var suggestions = results.Select(r => new
            {
                lookupId = r.LookupId,
                accountHeadKeyword = r.AccountHeadKeywords,
                accountTypeKeyword = r.AccountTypeKeywords,
                label = $"{r.AccountHeadKeywords} ({r.AccountTypeKeywords})",
                value = r.AccountHeadKeywords
            });

            return Json(suggestions);
        }

        // Toggle Active/Inactive only
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int accountHeadId, bool isActive, int pageNumber = 1)
        {
            try
            {
                var accountHead = await _repo.GetByIdAsync(accountHeadId);
                await _repo.UpdateStatusAsync(accountHeadId, isActive, ModelState);

                if (ModelState.IsValid)
                {
                    TempData["Message"] =
                        $"Account Head '{accountHead?.AccountHeadName}' (Code: {accountHead?.AccountHeadCode}) has been {(isActive ? "activated" : "deactivated")}.";
                }
                else
                {
                    TempData["Error"] = string.Join(" | ",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }
            }
            catch (SqlException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { pageNumber });
        }
    }
}
