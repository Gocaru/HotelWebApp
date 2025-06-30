using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class ReservationsController : Controller
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(IReservationRepository reservationRepo, IRoomRepository roomRepo, UserManager<ApplicationUser> userManager)
        {
            _reservationRepo = reservationRepo;
            _roomRepo = roomRepo;
            _userManager = userManager;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationRepo.GetAllWithDetailsAsync(); // Inclui User e Room
            return View(reservations);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id.Value);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public async Task<IActionResult> Create()
        {
            var model = new ReservationViewModel
            {
                Guests = await GetGuestListItems(),
                Rooms = await GetRoomListItems()
            };
            return View(model);
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationViewModel model)
        {
            await ValidateReservationModel(model);

            if (ModelState.IsValid)
            {
                var room = await _roomRepo.GetByIdAsync(model.RoomId);
                if (room == null) return NotFound();

                var numberOfNights = (model.CheckOutDate - model.CheckInDate).Days;

                var reservation = new Reservation
                {
                    GuestId = model.GuestId,
                    RoomId = model.RoomId,
                    CheckInDate = model.CheckInDate,
                    CheckOutDate = model.CheckOutDate,
                    Status = ReservationStatus.Confirmed,
                    TotalPrice = numberOfNights * room.PricePerNight
                };

                await _reservationRepo.CreateAsync(reservation);

                room.Status = RoomStatus.Reserved;
                await _roomRepo.UpdateAsync(room);

                return RedirectToAction(nameof(Index));
            }

            model.Guests = await GetGuestListItems();
            model.Rooms = await GetRoomListItems();
            return View(model);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id.Value);
            if (reservation == null)
            {
                return NotFound();
            }

            // Mapeia a entidade para o ViewModel para edição
            var model = new ReservationViewModel
            {
                Id = reservation.Id,
                GuestId = reservation.GuestId,
                RoomId = reservation.RoomId,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                Status = reservation.Status,
                Guests = await GetGuestListItems(),
                Rooms = await GetRoomListItems()
            };

            return View(model);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReservationViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            await ValidateReservationModel(model, model.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    var reservationToUpdate = await _reservationRepo.GetByIdWithDetailsAsync(id);
                    if (reservationToUpdate == null) return NotFound();

                    var room = await _roomRepo.GetByIdAsync(model.RoomId);
                    if (room == null) return NotFound();

                    var numberOfNights = (model.CheckOutDate - model.CheckInDate).Days;

                    // Atualiza as propriedades da entidade existente
                    reservationToUpdate.GuestId = model.GuestId;
                    reservationToUpdate.RoomId = model.RoomId;
                    reservationToUpdate.CheckInDate = model.CheckInDate;
                    reservationToUpdate.CheckOutDate = model.CheckOutDate;
                    reservationToUpdate.TotalPrice = numberOfNights * room.PricePerNight;
                    // O Status pode ser alterado aqui se o formulário permitir (ex: para cancelar)
                    // reservationToUpdate.Status = model.Status;

                    await _reservationRepo.UpdateAsync(reservationToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Se a validação falhar, repopula as dropdowns
            model.Guests = await GetGuestListItems();
            model.Rooms = await GetRoomListItems();
            return View(model);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id.Value);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);

            if (reservation == null) return RedirectToAction(nameof(Index));

            await _reservationRepo.DeleteAsync(id);

            // Regra de negócio: Após apagar/cancelar uma reserva, o quarto deve voltar a estar disponível
            // (Isto pode precisar de lógica mais complexa: só fica disponível se não houver outras reservas para ele)
            var room = await _roomRepo.GetByIdAsync(reservation.RoomId);
            if (room != null)
            {
                room.Status = RoomStatus.Available; // Simplificação, pode precisar de mais lógica
                await _roomRepo.UpdateAsync(room);
            }

            return RedirectToAction(nameof(Index));

        }

        private async Task<IEnumerable<SelectListItem>> GetGuestListItems()
        {
            var guests = await _userManager.GetUsersInRoleAsync("Guest");
            return guests.Select(u => new SelectListItem { Value = u.Id, Text = u.FullName ?? u.Email });
        }

        private async Task<IEnumerable<SelectListItem>> GetRoomListItems()
        {
            var rooms = await _roomRepo.GetAllAsync();
            return rooms.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.RoomNumber });
        }

        private async Task ValidateReservationModel(ReservationViewModel model, int? reservationId = null)
        {
            if (model.CheckOutDate <= model.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Check-out date must be after the check-in date.");
            }

            if (!await _reservationRepo.IsRoomAvailableAsync(model.RoomId, model.CheckInDate, model.CheckOutDate, reservationId))
            {
                ModelState.AddModelError("", "This room is not available for the selected dates.");
            }
        }
    }
}
