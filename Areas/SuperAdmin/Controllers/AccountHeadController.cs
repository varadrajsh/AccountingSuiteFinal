using AccountingSuite.Data;
using AccountingSuite.Infrastructure;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AccountingSuite.Models.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;


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
            if (!ModelState.IsValid)
            {
                var accountHeads = await _repo.GetAllAsync();
                var paginated = PaginatedList<AccountHead>.Create(accountHeads, 1, 10);
                return View("Index", paginated);
            }

            var result = await _repo.CreateAsync(accountHead, ModelState);

            if (result.newId > 0)
            {
                TempData["Message"] = $"Account Head '{accountHead.AccountHeadName}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            var accountHeadsReload = await _repo.GetAllAsync();
            var paginatedReload = PaginatedList<AccountHead>.Create(accountHeadsReload, 1, 10);
            return View("Index", paginatedReload);
        }

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

        // GET: AccountHead/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var accountHead = await _repo.GetByIdAsync(id);
            if (accountHead == null) return NotFound();

            return View(accountHead);
        }

        // POST: AccountHead/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AccountHead accountHead)
        {
            if (!ModelState.IsValid)
            {
                return View(accountHead);
            }

            try
            {
                _repo.UpdateStatus(accountHead.AccountHeadId, accountHead.IsActive);

                TempData["Message"] = "Account Head status updated successfully.";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState);
                return View(accountHead);
            }
        }
    }
}
