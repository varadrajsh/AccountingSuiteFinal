using AccountingSuite.Data;
using AccountingSuite.Models.Common;
using AccountingSuite.Models.Accounting;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSuite.Controllers
{
    public class AccountHeadController : Controller
    {
        private readonly AccountHeadRepository _repository;

        public AccountHeadController(AccountHeadRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index(string? searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var accountHeads = _repository.GetAll();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                accountHeads = accountHeads.Where(a =>
                    a.AccountHeadCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    a.AccountHeadName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    a.AccountHeadType.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
                ViewData["CurrentFilter"] = searchTerm;
            }

            var paginatedList = PaginatedList<AccountHead>.Create(accountHeads, pageNumber, pageSize);
            return View(paginatedList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AccountHead accountHead)
        {
            if (!ModelState.IsValid) return View(accountHead);

            try
            {
                accountHead.IsActive = true; // default active
                var result = _repository.Create(accountHead);
                TempData["Message"] = $"Account Head created successfully with Code: {result.code}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(accountHead);
            }
        }

        public IActionResult Edit(int id)
        {
            var accountHead = _repository.GetById(id);
            if (accountHead == null) return NotFound();
            return PartialView("_Edit", accountHead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AccountHead accountHead)
        {
            // if (!ModelState.IsValid) return PartialView("_Edit", accountHead);

            try
            {
                _repository.UpdateStatus(accountHead.AccountHeadId, accountHead.IsActive);
                TempData["Message"] = "Account Head status updated successfully.";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return PartialView("_Edit", accountHead);
            }
        }
    }
}
