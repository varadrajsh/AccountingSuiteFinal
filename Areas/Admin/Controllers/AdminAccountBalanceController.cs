using AccountingSuite.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSuite.Areas.Admin.Controllers
{
    public class AdminAccountBalanceController : Controller
    {
        private readonly AdminAccountBalanceService _service;
        public AdminAccountBalanceController(AdminAccountBalanceService service)
        {
            _service = service;
        }


        // GET: /AdminAccountBalance/Index
        public async Task<ActionResult> Index(int branchId, DateTime balanceData)
        {
            var balances = await _service.GetBalancesAsync(branchId, balanceData);
            return View(balances);
        }

        // POST: /AdminAccountBalance/Recalculate
        [HttpPost]
        public async Task<IActionResult> Recalculate(int branchId, DateTime balanceDate)
        {
            await _service.RecalculateClosingAsync(branchId, balanceDate);
            TempData["Message"] = "Closing balances recalculated successfully.";
            return RedirectToAction("Index", new { branchId, balanceDate });
        }

        // POST: /AdminAccountBalance/CarryForward
        [HttpPost]
        public async Task<IActionResult> CarryForward(int branchId, DateTime nextDate)
        {
            await _service.CarryForwardOpeningAsync(branchId, nextDate);
            TempData["Message"] = "Opening balances carried forward successfully.";
            return RedirectToAction("Index", new{branchId, balanceDate = nextDate});
        }
    }
}
