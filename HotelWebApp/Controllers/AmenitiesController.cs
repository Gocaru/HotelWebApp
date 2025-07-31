using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelWebApp.Controllers
{
    /// <summary>
    /// Controller for managing hotel amenities (extra services).
    /// Accessible only by users with the 'Admin' role.
    /// Handles all CRUD operations for the Amenity entity.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AmenitiesController : Controller
    {
        private readonly IAmenityRepository _amenityRepository;

        public AmenitiesController(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        // GET: Amenities
        /// <summary>
        /// Displays a list of all available amenities.
        /// </summary>
        /// <returns>The view with the list of amenities.</returns>
        public async Task<IActionResult> Index()
        {
            var amenities = await _amenityRepository.GetAllAsync();
            return View(amenities);
        }


        // GET: Amenities/Details/5
        /// <summary>
        /// Displays the details of a specific amenity.
        /// </summary>
        /// <param name="id">The ID of the amenity to display.</param>
        /// <returns>The details view or NotFound if the amenity doesn't exist.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _amenityRepository.GetByIdAsync(id.Value);
            if (amenity == null)
            {
                return NotFound();
            }

            return View(amenity);
        }

        // GET: Amenities/Create
        /// <summary>
        /// Displays the form to create a new amenity.
        /// </summary>
        /// <returns>The create view.</returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Amenities/Create
        /// <summary>
        /// Handles the submission of the new amenity form.
        /// Creates a new amenity in the database if the model is valid.
        /// </summary>
        /// <param name="amenity">The amenity object created from the form data.</param>
        /// <returns>Redirects to the Index view on success, otherwise redisplays the form with validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price")] Amenity amenity)
        {
            if (ModelState.IsValid)
            {
                await _amenityRepository.CreateAsync(amenity);
                TempData["SuccessMessage"] = "Amenity created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }

        // GET: Amenities/Edit/5
        /// <summary>
        /// Displays the form to edit an existing amenity.
        /// </summary>
        /// <param name="id">The ID of the amenity to edit.</param>
        /// <returns>The edit view with the amenity's data or NotFound.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _amenityRepository.GetByIdAsync(id.Value);
            if (amenity == null)
            {
                return NotFound();
            }
            return View(amenity);
        }

        // POST: Amenities/Edit/5
        /// <summary>
        /// Handles the submission of the amenity edit form.
        /// Updates the amenity in the database if the model is valid.
        /// </summary>
        /// <param name="id">The ID of the amenity being edited.</param>
        /// <param name="amenity">The amenity object with the updated values.</param>
        /// <returns>Redirects to the Index view on success, otherwise redisplays the form.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price")] Amenity amenity)
        {
            if (id != amenity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _amenityRepository.UpdateAsync(amenity);
                TempData["SuccessMessage"] = "Amenity updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }

        // GET: Amenities/Delete/5
        /// <summary>
        /// Displays the confirmation page before deleting an amenity.
        /// </summary>
        /// <param name="id">The ID of the amenity to delete.</param>
        /// <returns>The delete confirmation view or NotFound.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _amenityRepository.GetByIdAsync(id.Value);
            if (amenity == null)
            {
                return NotFound();
            }

            return View(amenity);
        }

        // POST: Amenities/Delete/5
        /// <summary>
        /// Deletes the specified amenity from the database after confirmation.
        /// Prevents deletion if the amenity is currently associated with any reservations.
        /// </summary>
        /// <param name="id">The ID of the amenity to delete.</param>
        /// <returns>Redirects to the Index view.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _amenityRepository.IsInUseAsync(id))
            {
                // Business rule: An amenity cannot be deleted if it's in use by any reservation.
                TempData["ErrorMessage"] = "This amenity cannot be deleted as it is linked to one or more reservations.";
                return RedirectToAction(nameof(Index));
            }

            var amenity = await _amenityRepository.GetByIdAsync(id);
            if (amenity != null)
            {
                await _amenityRepository.DeleteAsync(amenity);
                TempData["SuccessMessage"] = "Amenity deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
