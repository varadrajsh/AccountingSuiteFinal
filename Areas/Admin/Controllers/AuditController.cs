using AccountingSuite.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace AccountingSuite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuditController : Controller
    {
        private readonly AuditRepository _repo;
        public AuditController(AuditRepository repo)
        {
            _repo = repo;
        }

        // Admin can see only their approvals / disapprovals
        public async Task<IActionResult> Index(int adminId, string? modelName= null)
        {
            var audits = await _repo.GetAuditsReportAsync("Admin", adminId, modelName);
            return View(audits);
        }
    }
}
