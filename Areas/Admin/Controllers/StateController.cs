using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountingSuite.Data;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AccountingSuite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StateController : Controller
    {
        private readonly StateRepository _stateRepo;
        private readonly RegionRepository _regionRepo;

        public StateController(StateRepository stateRepo, RegionRepository regionRepo)
        {
            _stateRepo = stateRepo;
            _regionRepo = regionRepo;
        }
        public async Task<IActionResult> Index(int? regionId)
        {
            ViewBag.Regions = await _regionRepo.GetAllAsync();

            IEnumerable<State> states;
            if (regionId.HasValue && regionId.Value > 0)
            {
                states = await _stateRepo.GetByRegionAsync(regionId.Value);
                ViewBag.RegionId = regionId.Value;
            }
            else
            {
                states = await _stateRepo.GetAllAsync();
                ViewBag.RegionId = null;
            }
            return View(states);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync(int regionId)
        {
            ViewBag.Regions = await _regionRepo.GetAllAsync();
            ViewBag.RegionId = regionId;
            return PartialView("_CreatePartial", new State { RegionId = regionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(State state, int regionId)
        {
            state.StateName = state.StateName?.Trim().TrimEnd('.').ToUpperInvariant();
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data. Please check the form.";
                return RedirectToAction("Index", new { regionId });
            }

            try
            {
                await _stateRepo.InsertAsync(state);
                TempData["Message"] = "State added successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"State '{state.StateName}' already exists";
            }

            return RedirectToAction("Index", new { regionId });
        }
    }
}