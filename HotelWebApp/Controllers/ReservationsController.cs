using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Controllers
{ 
    public class ReservationsController : Controller
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReservationService _reservationService;
        private readonly IAmenityRepository _amenityRepository;

        public ReservationsController(IReservationRepository reservationRepo, IRoomRepository roomRepo, UserManager<ApplicationUser> userManager, IReservationService reservationService, IAmenityRepository amenityRepository)
        {
            _reservationRepo = reservationRepo;
            _roomRepo = roomRepo;
            _userManager = userManager;
            _reservationService = reservationService;
            _amenityRepository = amenityRepository;
        }

        // GET: Reservations
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationRepo.GetAllWithDetailsAsync(); // Inclui User e Room
            return View(reservations);
        }

        // GET: Reservations/Details/5
        [Authorize(Roles = "Employee, Guest")]
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

            if (User.IsInRole("Guest"))
            {
                var currentUserId = _userManager.GetUserId(User);
                if (reservation.GuestId != currentUserId)
                {
                    return Forbid(); // Acesso Proibido! A reserva não é deste Guest.
                }
            }

            if (User.IsInRole("Employee") || User.IsInRole("Admin"))
            {
                var allAmenities = await _amenityRepository.GetAllAsync();
                ViewBag.Amenities = new SelectList(allAmenities, "Id", "Name");
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        [Authorize(Roles = "Employee, Guest")]
        public async Task<IActionResult> Create()
        {
            var model = new ReservationViewModel();

            if (User.IsInRole("Employee"))
            {
                // Funcionário vê todos os quartos e todos os hóspedes
                model.Rooms = await GetRoomListItems();
                model.Guests = await GetGuestListItems();
            }
            else // É um Guest
            {
                // Hóspede só vê os quartos disponíveis.
                // método que retorna apenas quartos livres.
                model.Rooms = await GetAvailableRoomListItems();
            }


            model.CheckInDate = DateTime.Today.AddDays(1);
            model.CheckOutDate = DateTime.Today.AddDays(2);

            return View(model);
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee, Guest")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationViewModel model)
        {

            if (User.IsInRole("Guest"))
            {
                model.GuestId = _userManager.GetUserId(User);
            }
            else if (User.IsInRole("Employee"))
            {
                // Se for um funcionário e não selecionou um hóspede, adicionamos um erro ao ModelState.
                if (string.IsNullOrEmpty(model.GuestId))
                {
                    ModelState.AddModelError("GuestId", "Please select a guest for the reservation.");
                }
            }


            await ValidateReservationModel(model);

            if (ModelState.IsValid)
            {
                var result = await _reservationService.CreateReservationAsync(model, model.GuestId);

                if (result.Succeeded)
                {
                    // Redireciona com base no role do utilizador
                    if (User.IsInRole("Guest"))
                    {
                        return RedirectToAction(nameof(MyReservations));
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Se o serviço retornou um erro, mostre-o ao utilizador
                    ModelState.AddModelError(string.Empty, result.Error);
                }
            }

            if (User.IsInRole("Employee"))
            {
                model.Rooms = await GetRoomListItems();
                model.Guests = await GetGuestListItems();
            }

            else
            {
                model.Rooms = await GetAvailableRoomListItems();
            }

            return View(model);
        }

        // GET: Reservations/Edit/5
        [Authorize(Roles = "Employee, Guest")]
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
                NumberOfGuests = reservation.NumberOfGuests,
                Guests = await GetGuestListItems(),
                Rooms = await GetRoomListItems()
            };

            return View(model);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee, Guest")]
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
                var result = await _reservationService.UpdateReservationAsync(id, model);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Se o serviço retornou um erro, mostre-o
                    ModelState.AddModelError(string.Empty, result.Error);
                }
            }

            // Se a validação falhar, repopula as dropdowns
            model.Guests = await GetGuestListItems();
            model.Rooms = await GetRoomListItems();
            return View(model);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "Employee")]
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
        [Authorize(Roles = "Employee")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);

            if (reservation == null) return RedirectToAction(nameof(Index));

            await _reservationRepo.DeleteAsync(id);

            // Regra de negócio: Após apagar/cancelar uma reserva, o quarto deve voltar a estar disponível
            var room = await _roomRepo.GetByIdAsync(reservation.RoomId);
            if (room != null)
            {
                room.Status = RoomStatus.Available;
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

        private async Task<IEnumerable<SelectListItem>> GetAvailableRoomListItems()
        {
            // Usa o novo método do repositório
            var availableRooms = await _roomRepo.GetAvailableRoomsAsync();

            // Transforma a lista de quartos numa lista de itens para o dropdown
            return availableRooms.Select(r => new SelectListItem
            {
                //formata o texto para ser mais útil para o hóspede
                Text = $"Quarto {r.RoomNumber} ({r.Type}) - {r.PricePerNight:C}",
                Value = r.Id.ToString()
            });
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

            if (model.RoomId > 0 && model.NumberOfGuests > 0)
            {
                var room = await _roomRepo.GetByIdAsync(model.RoomId);
                if (room != null && model.NumberOfGuests > room.Capacity)
                {
                    ModelState.AddModelError("NumberOfGuests", $"The selected room only accommodates up to {room.Capacity} guests.");
                }
            }
        }

        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> MyReservations()
        {
            var userId = _userManager.GetUserId(User);

            var reservations = await _reservationRepo.GetReservationsByGuestIdWithDetailsAsync(userId);

            return View(reservations);
        }

        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id.Value);
            if (reservation == null) return NotFound();

            // Segurança: Garante que o Guest só pode cancelar as suas próprias reservas
            if (reservation.GuestId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            // Regra de Negócio: Garante que a reserva ainda pode ser cancelada
            if (reservation.CheckInDate <= DateTime.Today || reservation.Status != ReservationStatus.Confirmed)
            {
                TempData["ErrorMessage"] = "This reservation can no longer be cancelled.";
                return RedirectToAction(nameof(MyReservations));
            }

            return View(reservation);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var reservation = await _reservationRepo.GetByIdAsync(id);
            if (reservation == null) return NotFound();

            // Dupla verificação de segurança e regras
            if (reservation.GuestId != _userManager.GetUserId(User) || reservation.CheckInDate <= DateTime.Today)
            {
                return Forbid();
            }

            // Lógica de cancelamento
            reservation.Status = ReservationStatus.Cancelled;
            await _reservationRepo.UpdateAsync(reservation);

            // Lógica para libertar o quarto
            var room = await _roomRepo.GetByIdAsync(reservation.RoomId);
            if (room != null)
            {
                room.Status = RoomStatus.Available; // Simplificação: assume que o quarto fica logo livre
                await _roomRepo.UpdateAsync(room);
            }

            TempData["SuccessMessage"] = "Reservation cancelled successfully.";
            return RedirectToAction(nameof(MyReservations));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CheckIn(int id)
        {
            // Obtem a reserva com os detalhes necessários (incluindo o quarto)
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            // Valida as regras de negócio para o check-in
            // Só se pode fazer check-in de uma reserva 'Confirmed' e na data de entrada.
            if (reservation.Status != ReservationStatus.Confirmed)
            {
                TempData["ErrorMessage"] = "Apenas reservas confirmadas podem fazer check-in.";
                return RedirectToAction(nameof(Index));
            }

            if (reservation.CheckInDate.Date != DateTime.Today.Date)
            {
                TempData["ErrorMessage"] = "O check-in só pode ser realizado na data de entrada da reserva.";
                return RedirectToAction(nameof(Index));
            }

            // Atualiza os status da Reserva e do Quarto
            reservation.Status = ReservationStatus.CheckedIn;
            if (reservation.Room != null)
            {
                reservation.Room.Status = RoomStatus.Occupied;
            }
            else
            {
                // Se por algum motivo o quarto não estiver associado, não podemos continuar.
                TempData["ErrorMessage"] = "Erro: A reserva não tem um quarto associado.";
                return RedirectToAction(nameof(Index));
            }

            // Salva as alterações
            await _reservationRepo.UpdateAsync(reservation);

            TempData["SuccessMessage"] = $"Check-in para o hóspede {reservation.ApplicationUser.FullName} realizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            // Valida a regra de negócio: só se pode fazer check-out de uma reserva que está "CheckedIn"
            if (reservation.Status != ReservationStatus.CheckedIn)
            {
                TempData["ErrorMessage"] = "This action is only available for currently checked-in reservations.";
                return RedirectToAction(nameof(Index));
            }

            // Atualiza os status da Reserva para "CheckedOut" e do Quarto para "Available"
            reservation.Status = ReservationStatus.CheckedOut;
            if (reservation.Room != null)
            {
                reservation.Room.Status = RoomStatus.Available;
            }
            else
            {
                // Medida de segurança
                TempData["ErrorMessage"] = "Error: The reservation is not linked to a room.";
                return RedirectToAction(nameof(Index));
            }

            // TODO (Conforme o enunciado):
            // Neste ponto, seria implementada a lógica de "Cálculo automático do valor total" e "Geração de faturas".
            // Por exemplo: reservation.FinalPrice = CalculateFinalPrice(reservation);

            // Salva as alterações na base de dados
            await _reservationRepo.UpdateAsync(reservation);

            TempData["SuccessMessage"] = $"Check-out for guest '{reservation.ApplicationUser.FullName}' completed successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> AddAmenityToReservation(int reservationId, int amenityId, int quantity)
        {
            var result = await _reservationService.AddAmenityToReservationAsync(reservationId, amenityId, quantity);

            if (!result.Succeeded)
            {
                // Se houve um erro no serviço, guarda a mensagem de erro para ser exibida
                // na página de detalhes e redireciona de volta.
                TempData["ErrorMessage"] = result.Error;
            }
            else
            {
                TempData["SuccessMessage"] = "Amenity added successfully!";
            }

            // Redireciona de volta para a página de detalhes da mesma reserva
            return RedirectToAction(nameof(Details), new { id = reservationId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> RemoveAmenityFromReservation(int reservationId, int reservationAmenityId)
        {
            var result = await _reservationService.RemoveAmenityFromReservationAsync(reservationId, reservationAmenityId);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Error;
            }
            else
            {
                TempData["SuccessMessage"] = "Amenity removed successfully!";
            }

            return RedirectToAction(nameof(Details), new { id = reservationId });
        }

    }
}
