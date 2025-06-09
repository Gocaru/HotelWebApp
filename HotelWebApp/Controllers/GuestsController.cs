using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelWebApp.Controllers
{
    public class GuestsController : Controller
    {
        private readonly IGuestRepository _guestRepository;

        public GuestsController(IGuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        // GET: GuestsController
        public async Task<IActionResult> Index()
        {
            var guests = await _guestRepository.GetAllAsync();
            return View(guests);
        }

        // GET: GuestsController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var guest = await _guestRepository.GetByIdAsync(id);
            if(guest == null)
                return NotFound();

            return View(guest);
        }

        // GET: GuestsController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GuestsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guest guest)
        {
            if(ModelState.IsValid)
            {
                await _guestRepository.AddAsync(guest);
                return RedirectToAction(nameof(Index));
            }

            return View(guest);
        }

        // GET: GuestsController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var guest = await _guestRepository.GetByIdAsync(id);
            if (guest == null)
                return NotFound();

            return View(guest);
        }

        // POST: GuestsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Guest guest)
        {
            if (id != guest.Id)
                return NotFound();

            if(ModelState.IsValid)
            {
                var exists = await _guestRepository.ExistsAsync(id);
                if(!exists)
                    return NotFound();

                await _guestRepository.UpdateAsync(guest);
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        // GET: GuestsController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var guest = await _guestRepository.GetByIdAsync(id);
            if (guest == null)
                return NotFound();

            return View(guest);
        }

        // POST: GuestsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _guestRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
