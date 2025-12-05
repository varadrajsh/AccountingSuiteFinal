using AccountingSuite.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSuite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminDayLockController : Controller
    {
        private readonly AdminDayLockService _service;
        public AdminDayLockController(AdminDayLockService service)
        {
            _service = service;
        }


        // GET: Admin/AdminDayLock/
        public ActionResult Index()
        {
            return View();
        }

    }
}
