using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> Index(int? regionId)
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
            state.StateName = state.StateName?.Trim().TrimEnd('.').ToUpperInvariant();
            if (!ModelState.IsValid)
            {
                await PopulateRegionsDropdown();
                return View(state);
            }

            try
            {
                var result = await _stateRepo.InsertAsync(state);

                // Map stored procedure status to friendly message
                TempData["Message"] = result.Status;
                if (result.Status != "SUCCESS")
                {
                    await PopulateRegionsDropdown();
                    return View(state);
                }

                return RedirectToAction("Index", new { regionId = state.RegionId });
            }
            catch (SqlException ex)
            {
                // Map SQL exceptions to ModelState errors
                SqlErrorMapper.Map(ex, ModelState);
                TempData["Error"] = "Database error while adding state.";
                await PopulateRegionsDropdown();
                return View(state);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unexpected application error: " + ex.Message;
                await PopulateRegionsDropdown();
                return View(state);
            }
        }

        // Show Edit form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var state = await _stateRepo.GetByIdAsync(id);
            if (state == null)
            {
                TempData["Error"] = "State not found.";
                return RedirectToAction("Index");
            }

            await PopulateRegionsDropdown();
            return View(state);
        }

        // Handle Edit submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(State state)
        {
            state.StateName = state.StateName?.Trim().TrimEnd('.').ToUpperInvariant();
            if (!ModelState.IsValid)
            {
                await PopulateRegionsDropdown();
                return View(state);
            }

            try
            {
                // Implement UpdateAsync in StateRepository similar to Party
                var result = await _stateRepo.InsertAsync(state); // placeholder for UpdateAsync
                TempData["Message"] = "State updated Successfully";

                if (result.Status != "SUCCESS")
                {
                    await PopulateRegionsDropdown();
                    return View(state);
                }

                return RedirectToAction("Index", new { regionId = state.RegionId });
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState);
                TempData["Error"] = "Database error while updating state.";
                await PopulateRegionsDropdown();
                return View(state);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unexpected application error: " + ex.Message;
                await PopulateRegionsDropdown();
                return View(state);
            }
        }

        // Details partial
        public async Task<IActionResult> Details(int id)
        {
            var state = await _stateRepo.GetByIdAsync(id);
            if (state == null) return NotFound();

            return PartialView("_DetailsPartial", state);
        }

        // Delete partial
        public async Task<IActionResult> Delete(int id)
        {
            var state = await _stateRepo.GetByIdAsync(id);
            if (state == null) return NotFound();

            return PartialView("_DeletePartial", state);
        }

        // Handle Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int StateId)
        {
            try
            {
                // Implement DeleteAsync in StateRepository similar to Party
                TempData["Message"] = "State deleted successfully.";
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, ModelState);
                TempData["Error"] = "Unable to delete State due to database constraint.";
            }
            return RedirectToAction("Index");
        }
    }
}
