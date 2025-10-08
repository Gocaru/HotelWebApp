using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelWebApp.Controllers
{
    /// <summary>
    /// Manages hotel activities that guests can book (Admin/Employee interface)
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class ActivitiesController : Controller
    {
        private readonly IActivityRepository _activityRepo;

        public ActivitiesController(IActivityRepository activityRepo)
        {
            _activityRepo = activityRepo;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
        {
            var activities = await _activityRepo.GetAllAsync();
            return View(activities);
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _activityRepo.GetByIdAsync(id.Value);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // GET: Activities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Activities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Duration,Schedule,Capacity,IsActive,ImageUrl")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                await _activityRepo.CreateAsync(activity);
                TempData["SuccessMessage"] = "Activity created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        // GET: Activities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _activityRepo.GetByIdAsync(id.Value);
            if (activity == null)
            {
                return NotFound();
            }
            return View(activity);
        }

        // POST: Activities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Duration,Schedule,Capacity,IsActive,ImageUrl")] Activity activity)
        {
            if (id != activity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _activityRepo.UpdateAsync(activity);
                TempData["SuccessMessage"] = "Activity updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _activityRepo.GetByIdAsync(id.Value);
            if (activity == null)
            {
                return NotFound();
            }

            // Check if activity has active bookings
            if (await _activityRepo.HasActiveBookingsAsync(id.Value))
            {
                TempData["ErrorMessage"] = "Cannot delete activity with active bookings.";
                return RedirectToAction(nameof(Index));
            }

            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _activityRepo.HasActiveBookingsAsync(id))
            {
                TempData["ErrorMessage"] = "Cannot delete activity with active bookings.";
                return RedirectToAction(nameof(Index));
            }

            await _activityRepo.DeleteAsync(id);
            TempData["SuccessMessage"] = "Activity deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
