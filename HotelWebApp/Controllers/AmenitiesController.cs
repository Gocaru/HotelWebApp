using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AmenitiesController : Controller
    {
        private readonly IAmenityRepository _amenityRepository;

        public AmenitiesController(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        // GET: Amenities
        public async Task<IActionResult> Index()
        {
            var amenities = await _amenityRepository.GetAllAsync();
            return View(amenities);
        }

        // GET: Amenities/Details/5
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Amenities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price")] Amenity amenity)
        {
            if (ModelState.IsValid)
            {
                await _amenityRepository.CreateAsync(amenity);
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }

        // GET: Amenities/Edit/5
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
                try
                {
                    await _amenityRepository.UpdateAsync(amenity);
                }
                catch (Exception)
                {
                    if (!await AmenityExists(amenity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }

        // GET: Amenities/Delete/5
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var amenity = await _amenityRepository.GetByIdAsync(id);
            if (amenity != null)
            {
                await _amenityRepository.DeleteAsync(amenity);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AmenityExists(int id)
        {
            return await _amenityRepository.AmenityExistsAsync(id);
        }
    }
}
