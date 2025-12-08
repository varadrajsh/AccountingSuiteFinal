using AccountingSuite.Data.Repositories;
using AccountingSuite.Models.Audit;
using AccountingSuite.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSuite.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class AuditController : Controller
    {
        private readonly AuditRepository _repo;

        public AuditController(AuditRepository repo)
        {
            _repo = repo;
        }

        // SuperAdmin Index: view audit records with filters + pagination
        public async Task<IActionResult> Index(
     string? branchSearch,
     DateTime? fromDate,
     DateTime? toDate,
     int? transactionId,
     string? modelName,
     int pageNumber = 1,
     int pageSize = 20)
        {
            var pagedAudits = await _repo.GetAuditsReportAsync(
                role: "SuperAdmin",
                userId: 0,
                modelName: modelName,
                branchSearch: branchSearch,
                transactionId: transactionId,
                fromDate: fromDate,
                toDate: toDate,
                pageNumber: pageNumber,
                pageSize: pageSize);

            return View(pagedAudits);
        }

    }
}
