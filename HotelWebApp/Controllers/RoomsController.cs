using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelWebApp.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IReservationRepository _reservationRepository;

        public RoomsController(IRoomRepository roomRepository, IReservationRepository reservationRepository)
        {
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
        }

        // GET: RoomsController
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return View(rooms);
        }

        // GET: RoomsController/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // GET: RoomsController/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var viewModel = new RoomViewModel();
            PopulateDropdowns();
            return View(viewModel);
        }

        // POST: RoomsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var roomToEdit = await _roomRepository.GetByIdAsync(id);

            if (roomToEdit == null)
            {
                return NotFound();
            }

            // Verifica se o quarto tem reservas futuras ou ativas.
            bool hasActiveOrFutureReservations = await _reservationRepository.HasFutureReservationsAsync(id);

            // Se tiver, bloquear a edição e informar o admin.
            if (hasActiveOrFutureReservations)
            {
                TempData["ErrorMessage"] = $"The Room '{roomToEdit.RoomNumber}' cannot be edited because it has active or future reservations.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new RoomViewModel
            {
                Id = roomToEdit.Id,
                RoomNumber = roomToEdit.RoomNumber,
                Capacity = roomToEdit.Capacity,
                PricePerNight = roomToEdit.PricePerNight,
                Type = roomToEdit.Type,
                Status = roomToEdit.Status
            };

            PopulateDropdowns();
            return View(viewModel);
        }

        // POST: RoomsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // POST: RoomsController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasFutureReservations = await _reservationRepository.HasFutureReservationsAsync(id);

            // Se tiver, bloquear a exclusão e informar o admin.
            if (hasFutureReservations)
            {
                // Pega os detalhes do quarto para usar na mensagem de erro.
                var room = await _roomRepository.GetByIdAsync(id);

                // Adiciona uma mensagem de erro clara ao TempData para ser exibida na página Index.
                TempData["ErrorMessage"] = $"The Room '{room?.RoomNumber}' cannot be deleted because it has future reservations scheduled.";

                // Redireciona de volta para a lista de quartos.
                return RedirectToAction(nameof(Index));
            }

            await _roomRepository.DeleteAsync(id);

            TempData["SuccessMessage"] = "Room deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        private void PopulateDropdowns()
        {
            ViewBag.RoomTypes = Enum.GetValues(typeof(RoomType));
            ViewBag.RoomStatuses = Enum.GetValues(typeof(RoomStatus));
        }
    }
}
