using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoomsController : Controller
    {
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        // GET: RoomsController
        public async Task<IActionResult> Index()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return View(rooms);
        }

        // GET: RoomsController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // GET: RoomsController/Create
        public IActionResult Create()
        {
            var viewModel = new RoomViewModel();
            PopulateDropdowns();
            return View();
        }

        // POST: RoomsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var room = new Room
                {
                    RoomNumber = viewModel.RoomNumber,
                    Capacity = viewModel.Capacity,
                    PricePerNight = viewModel.PricePerNight,
                    Type = viewModel.Type.Value,
                    Status = viewModel.Status.Value
                };

                await _roomRepository.AddAsync(room);
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns();
            return View(viewModel);
        }

        // GET: RoomsController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();

            var viewModel = new RoomViewModel
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Capacity = room.Capacity,
                PricePerNight = room.PricePerNight,
                Type = room.Type,
                Status = room.Status
            };

            PopulateDropdowns();
            return View(viewModel);
        }

        // POST: RoomsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var room = await _roomRepository.GetByIdAsync(id);

                if (room == null) return NotFound();

                room.RoomNumber = viewModel.RoomNumber;
                room.Capacity = viewModel.Capacity;
                room.PricePerNight = viewModel.PricePerNight;
                room.Type = viewModel.Type.Value;
                room.Status = viewModel.Status.Value;

                await _roomRepository.UpdateAsync(room);
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns();
            return View(viewModel);
        }

        // GET: RoomsController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // POST: RoomsController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _roomRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns()
        {
            ViewBag.RoomTypes = Enum.GetValues(typeof(RoomType));
            ViewBag.RoomStatuses = Enum.GetValues(typeof(RoomStatus));
        }
    }
}
