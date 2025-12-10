using System.Collections.Generic;
using System.Threading.Tasks;
using AccountingSuite.Data;
using AccountingSuite.Data.Repositories;
using AccountingSuite.Infrastructure;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        private async Task PopulateRegionsDropdown()
        {
            var regions = await _regionRepo.GetAllAsync();
            ViewBag.Regions = new SelectList(regions, "RegionId", "RegionName");
        }

        // Index with optional region filter
        public async Task<IActionResult> Index(int? regionId, int? newStateId)
        {
            ViewBag.Regions = await _regionRepo.GetAllAsync();

            IEnumerable<State> states;
            if (regionId.HasValue && regionId.Value > 0)
            {
                states = await _stateRepo.GetByRegionAsync(regionId.Value);
                ViewData["SelectedRegion"] = regionId.Value;
            }
            else
            {
                states = await _stateRepo.GetAllAsync();
                ViewData["SelectedRegion"] = null;
            }

            ViewData["NewStateId"] = newStateId;
            return View(states);
        }

        // Show Create form
        [HttpGet]
        public async Task<IActionResult> Create(int regionId)
        {
            await PopulateRegionsDropdown();
            return View(new State { RegionId = regionId });
        }

        // Handle Create submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(State state)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRegionsDropdown();
                return View(state);
            }

            try
            {
                var result = await _stateRepo.InsertAsync(state);

                if (result.Status == "DUPLICATE_REGION")
                {
                    ModelState.AddModelError("StateName", "State already exists in this region.");
                    await PopulateRegionsDropdown();
                    return View(state);
                }

                if (result.Status == "DUPLICATE_OTHER_REGION")
                {
                    ModelState.AddModelError("StateName", "State already exists in another region.");
                    await PopulateRegionsDropdown();
                    return View(state);
                }

                if (result.Status != "SUCCESS")
                {
                    ModelState.AddModelError("", "Unable to add state. " + result.Status);
                    await PopulateRegionsDropdown();
                    return View(state);
                }

                TempData["Message"] = "State Added Successfully";
                return RedirectToAction("Index", new { regionId = state.RegionId, newStateId = result.NewId });
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState); // rely on ModelState for inline errors
                await PopulateRegionsDropdown();
                return View(state);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Unexpected application error: " + ex.Message);
                await PopulateRegionsDropdown();
                return View(state);
            }
        }
    }
}
