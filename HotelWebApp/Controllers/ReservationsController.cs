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
        private readonly IPaymentService _paymentService;

        public ReservationsController(IReservationRepository reservationRepo, IRoomRepository roomRepo, UserManager<ApplicationUser> userManager, IReservationService reservationService, IAmenityRepository amenityRepository, IPaymentService paymentService)
        {
            _reservationRepo = reservationRepo;
            _roomRepo = roomRepo;
            _userManager = userManager;
            _reservationService = reservationService;
            _amenityRepository = amenityRepository;
            _paymentService = paymentService;
        }

        // GET: Reservations
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationRepo.GetAllWithDetailsAsync(); // Inclui User, Room e amenities

            var viewModelList = reservations.Select(res =>
            {
                // 1. Calcular o custo base da estadia
                // Garante que estadias de 0 dias (ex: check-in e out no mesmo dia) contam como 1 noite.
                int numberOfNights = (res.CheckOutDate.Date - res.CheckInDate.Date).Days;
                if (numberOfNights <= 0)
                {
                    numberOfNights = 1;
                }

                decimal stayCost = numberOfNights * (res.Room?.PricePerNight ?? 0);

                // 2. Calcular o custo total dos amenities
                // A propriedade res.ReservationAmenities vem preenchida graças ao .Include() no repositório
                decimal amenitiesCost = res.ReservationAmenities?.Sum(ra => (ra.Amenity?.Price ?? 0) * ra.Quantity) ?? 0;

                // 3. Retornar o ViewModel preenchido
                return new ReservationListViewModel
                {
                    Id = res.Id,
                    GuestName = res.ApplicationUser?.FullName ?? "N/A",
                    RoomNumber = res.Room?.RoomNumber ?? "N/A",
                    CheckInDate = res.CheckInDate,
                    CheckOutDate = res.CheckOutDate,
                    StatusText = res.Status.ToString(),
                    StatusBadgeClass = GetBadgeClassForStatus(res.Status),
                    NumberOfGuests = res.NumberOfGuests,
                    RoomDetails = $"{res.Room?.RoomNumber} ({res.Room?.Type})",
                    TotalCost = stayCost + amenitiesCost,

                    // Lógica para os botões de ação (inalterada)
                    CanCheckIn = (res.Status == ReservationStatus.Confirmed && res.CheckInDate.Date <= DateTime.Today.Date),
                    CanCheckOut = (res.Status == ReservationStatus.CheckedIn),
                    CanEdit = (res.Status == ReservationStatus.Confirmed || res.Status == ReservationStatus.CheckedIn),
                    CanDelete = (res.Status == ReservationStatus.Confirmed)
                };
            }).OrderBy(r => r.CheckInDate).ToList();

            return View(viewModelList);
        }

        // Método helper privado para a lógica das cores
        private string GetBadgeClassForStatus(ReservationStatus status)
        {
            return status switch
            {
                ReservationStatus.Confirmed => "bg-primary",
                ReservationStatus.CheckedIn => "bg-success",
                ReservationStatus.Cancelled => "bg-danger",
                ReservationStatus.CheckedOut => "bg-dark",
                _ => "bg-secondary"
            };
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

            // Lógica de segurança para garantir que um Guest só edita as suas próprias reservas
            if (User.IsInRole("Guest") && reservation.GuestId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            // Regra de negócio: Não permitir edição se a reserva já foi concluída ou cancelada.
            // Isto impede o acesso à página de edição através do URL.
            if (reservation.Status == ReservationStatus.CheckedOut || reservation.Status == ReservationStatus.Cancelled)
            {
                TempData["ErrorMessage"] = $"Cannot edit a reservation with status '{reservation.Status}'.";
                return RedirectToAction(nameof(Index)); // Ou para a página de detalhes da reserva
            }

            // 1. Obter as listas completas de Hóspedes e Quartos
            var allGuestsList = await GetGuestListItems();
            var allRoomsList = await GetRoomListItems();

            // 2. Pré-selecionar o Hóspede correto na lista
            var selectedGuests = allGuestsList.Select(g =>
            {
                g.Selected = (g.Value == reservation.GuestId);
                return g;
            });

            // 3. Pré-selecionar o Quarto correto na lista
            var selectedRooms = allRoomsList.Select(r =>
            {
                r.Selected = (r.Value == reservation.RoomId.ToString());
                return r;
            });

            // 4. Mapear a entidade para o ViewModel com as listas já pré-selecionadas
            var model = new ReservationViewModel
            {
                Id = reservation.Id,
                GuestId = reservation.GuestId,
                RoomId = reservation.RoomId,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                Status = reservation.Status,
                NumberOfGuests = reservation.NumberOfGuests,
                Guests = selectedGuests, // Usar a lista com o item selecionado
                Rooms = selectedRooms     // Usar a lista com o item selecionado
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

            if (reservationId == null && model.CheckInDate.Date < DateTime.Today.Date)
            {
                ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past.");
            }

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
        public async Task<IActionResult> CheckIn(int id, string source = null)
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
                TempData["ErrorMessage"] = "Only confirmed reservations can be checked in.";
                return source == "dashboard" ? RedirectToAction("Index", "Home") : RedirectToAction(nameof(Index));
            }

            if (reservation.CheckInDate.Date != DateTime.Today.Date)
            {
                TempData["ErrorMessage"] = "Check-in can only be done on or after the reservation date.";
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
                TempData["ErrorMessage"] = "Error: The reservation does not have an associated room.";
                return source == "dashboard" ? RedirectToAction("Index", "Home") : RedirectToAction(nameof(Index));
            }

            // Salva as alterações
            await _reservationRepo.UpdateAsync(reservation);

            TempData["SuccessMessage"] = $"Check-in for guest {reservation.ApplicationUser.FullName} completed successfully!";
            return RedirectToAction(nameof(Index));

            //lógica de redirecionamento inteligente:
            if (source == "dashboard")
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CheckOut(int id, string source = null)
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
                return source == "dashboard" ? RedirectToAction("Index", "Home") : RedirectToAction(nameof(Index));
            }

            // Chamamos o serviço para gerar a fatura.
            var invoiceResult = await _paymentService.CreateInvoiceForReservationAsync(id);

            // Verificamos se a criação da fatura correu bem
            if (!invoiceResult.Succeeded)
            {
                // Se algo correu mal
                // mostramos o erro ao funcionário.
                TempData["ErrorMessage"] = invoiceResult.Error;
                return source == "dashboard" ? RedirectToAction("Index", "Home") : RedirectToAction(nameof(Index));
            }

            // Atualiza os status da Reserva para "CheckedOut" e do Quarto para "Available"
            reservation.Status = ReservationStatus.CheckedOut;
            if (reservation.Room != null)
            {
                reservation.Room.Status = RoomStatus.Available;
            }
            
            await _reservationRepo.UpdateAsync(reservation);


            TempData["SuccessMessage"] = $"Check-out for guest '{reservation.ApplicationUser.FullName}' completed. Please process the payment.";

            //Redirecionamos o funcionário para a página de detalhes da fatura que acabámos de criar.
            return RedirectToAction("Details", "Invoices", new { id = invoiceResult.Data.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> AddAmenityToReservation(int reservationId, int amenityId, int quantity)
        {
            var reservation = await _reservationRepo.GetByIdAsync(reservationId);
            if (reservation == null) return NotFound();

            if (reservation.Status != ReservationStatus.Confirmed && reservation.Status != ReservationStatus.CheckedIn)
            {
                TempData["ErrorMessage"] = $"Cannot add services to a reservation with status '{reservation.Status}'.";
                return RedirectToAction(nameof(Details), new { id = reservationId });
            }

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
            var reservation = await _reservationRepo.GetByIdAsync(reservationId);
            if (reservation == null) return NotFound();

            if (reservation.Status != ReservationStatus.Confirmed && reservation.Status != ReservationStatus.CheckedIn)
            {
                TempData["ErrorMessage"] = $"Cannot remove services from a reservation with status '{reservation.Status}'.";
                return RedirectToAction(nameof(Details), new { id = reservationId });
            }

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
