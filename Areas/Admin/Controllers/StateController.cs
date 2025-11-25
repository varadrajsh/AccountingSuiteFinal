using System.Collections.Generic;
using System.Linq;
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

        // ✅ List states by region
        // public IActionResult Index(int? regionId)
        // {
        //     var regions = _regionRepo.GetAll();
        //     ViewBag.Regions = regions;

        //     IEnumerable<State> states = regionId.HasValue
        //         ? _stateRepo.GetByRegion(regionId.Value)
        //         : Enumerable.Empty<State>();

        //     ViewBag.RegionId = regionId;
        //     return View(states);
        // }

        public IActionResult Index(int? regionId)
        {
            ViewBag.Regions = _regionRepo.GetAll();

            IEnumerable<State> states;
            if (regionId.HasValue && regionId.Value > 0)
            {
                states = _stateRepo.GetByRegion(regionId.Value);
                ViewBag.RegionId = regionId.Value;
            }
            else
            {
                // ✅ Show all states across all regions
                states = _stateRepo.GetAll();
                ViewBag.RegionId = null;
            }

            return View(states);
        }


        // ✅ Show Create modal
        [HttpGet]
        public IActionResult Create(int regionId)
        {
            ViewBag.Regions = _regionRepo.GetAll();
            ViewBag.RegionId = regionId;
            return PartialView("_CreatePartial", new State { RegionId = regionId });
        }

        // ✅ Handle Create submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(State state, int regionId)
        {
            state.StateName = state.StateName?.Trim().TrimEnd('.').ToUpperInvariant();
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data. Please check the form.";
                return RedirectToAction("Index", new { regionId });
            }

            try
            {
                _stateRepo.Insert(state);
                TempData["Message"] = "State added successfully!";
            }
            catch (Exception ex)
            {
                // ✅ Any error (duplicate, SQL, etc.) → show in Index
                TempData["Error"] = $"State '{state.StateName}' already exists";
            }

            return RedirectToAction("Index", new { regionId });
        }


    }
}