using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        private readonly IChangeRequestRepository _changeRequestRepo;

        public ReservationsController(IReservationRepository reservationRepo,
                                        IRoomRepository roomRepo,
                                        UserManager<ApplicationUser> userManager,
                                        IReservationService reservationService,
                                        IAmenityRepository amenityRepository,
                                        IPaymentService paymentService,
                                        IChangeRequestRepository changeRequestRepo)
        {
            _reservationRepo = reservationRepo;
            _roomRepo = roomRepo;
            _userManager = userManager;
            _reservationService = reservationService;
            _amenityRepository = amenityRepository;
            _paymentService = paymentService;
            _changeRequestRepo = changeRequestRepo;
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
                    GuestDetails = $"{res.ApplicationUser?.FullName ?? "N/A"} ({res.NumberOfGuests} pax)",
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
                ReservationStatus.Completed => "bg-info",
                ReservationStatus.NoShow => "bg-warning",
                _ => "bg-secondary"
            };
        }

        // GET: Reservations/Details/5
        [Authorize(Roles = "Employee, Guest")]
        public async Task<IActionResult> Details(int? id, string source)
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

            //Prepara o ViewModel para a View
            var model = new ReservationDetailsViewModel
            {
                Reservation = reservation,
                ChangeRequests = await _changeRequestRepo.GetRequestsForReservationAsync(id.Value)
            };

            if (User.IsInRole("Employee"))
            {
                var allAmenities = await _amenityRepository.GetAllAsync();
                ViewBag.Amenities = new SelectList(allAmenities, "Id", "Name");
            }

            ViewBag.Source = source;

            return View(model);
        }

        // GET: Reservations/Create
        [Authorize(Roles = "Employee, Guest")]
        public async Task<IActionResult> Create(int? roomId, DateTime? checkInDate, DateTime? checkOutDate, string source)
        {
            var model = new ReservationViewModel();

            if (checkInDate.HasValue && checkOutDate.HasValue)
            {
                if (checkOutDate.Value <= checkInDate.Value)
                {
                    // Se as datas são inválidas, adiciona um erro ao ModelState
                    ModelState.AddModelError("CheckOutDate", "Check-out date must be after the check-in date.");

                    // Define as datas no modelo para que o utilizador as veja nos campos
                    model.CheckInDate = checkInDate.Value;
                    model.CheckOutDate = checkOutDate.Value;

                    // Retorna a View imediatamente, sem pesquisar quartos. O erro será mostrado.
                    return View(model);
                }
            }

            // Se um roomId foi passado, pré-seleciona-o
            if (roomId.HasValue)
            {
                model.RoomId = roomId.Value;
                var selectedRoom = await _roomRepo.GetByIdAsync(roomId.Value);
                if (selectedRoom != null)
                {
                    model.Rooms = new List<SelectListItem>
            {
                new SelectListItem { Text = $"Room {selectedRoom.RoomNumber} ({selectedRoom.Type}) - {selectedRoom.PricePerNight:C}", Value = selectedRoom.Id.ToString() }
            };
                }
            }
            // Se não há quarto, mas há datas, pesquisa os quartos disponíveis
            else if (checkInDate.HasValue && checkOutDate.HasValue)
            {
                var availableRooms = await _roomRepo.GetAvailableRoomsAsync(checkInDate.Value, checkOutDate.Value);
                model.Rooms = availableRooms.Select(r => new SelectListItem
                {
                    Text = $"Room {r.RoomNumber} ({r.Type}) - {r.PricePerNight:C}",
                    Value = r.Id.ToString()
                });
            }

            // Se for funcionário, carrega a lista de hóspedes e a lista completa de quartos se necessário
            if (User.IsInRole("Employee"))
            {
                model.Guests = await GetGuestListItems();
                if (model.Rooms == null || !model.Rooms.Any())
                {
                    model.Rooms = await GetRoomListItems();
                }
            }

            // Define as datas (as que vieram do URL ou as padrão)
            model.CheckInDate = checkInDate ?? DateTime.Today.AddDays(1);
            model.CheckOutDate = checkOutDate ?? DateTime.Today.AddDays(2);

            // Preserva o 'source'
            ViewBag.Source = source;

            if (model.CheckInDate > DateTime.MinValue)
            {
                ViewBag.TriedSearch = true;
            }

            return View(model);
        }


        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee, Guest")]
        public async Task<IActionResult> Create(ReservationViewModel model) 
        {

            if (User.IsInRole("Guest"))
            {
                model.GuestId = _userManager.GetUserId(User);
            }
            else if (User.IsInRole("Employee"))
            {
                if (string.IsNullOrEmpty(model.GuestId))
                {
                    ModelState.AddModelError("GuestId", "Please select a guest for the reservation.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Se a validação falhar, precisamos de recarregar as listas para a dropdown
                if (model.RoomId > 0 && (model.Rooms == null || !model.Rooms.Any()))
                {
                    // recarrega os dados do quarto pré-selecionado
                }
                else if (User.IsInRole("Employee"))
                {
                    model.Rooms = await GetRoomListItems();
                }
                // Os Guests para o Employee precisam sempre de ser recarregados
                if (User.IsInRole("Employee")) model.Guests = await GetGuestListItems();

                return View(model);
            }

            await ValidateReservationModel(model);

            if (ModelState.IsValid)
            {
                var result = await _reservationService.CreateReservationAsync(model, model.GuestId);

                if (result.Succeeded)
                {
                    return User.IsInRole("Guest") ? RedirectToAction(nameof(MyReservations)) : RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Error);
                }
            }

            // Recarrega os dados necessários para a View.
            if (User.IsInRole("Employee"))
            {
                model.Rooms = await GetRoomListItems();
                model.Guests = await GetGuestListItems();
            }
            else // Guest
            {
                if (model.CheckInDate > DateTime.MinValue && model.CheckOutDate > model.CheckInDate)
                {
                    var availableRooms = await _roomRepo.GetAvailableRoomsAsync(model.CheckInDate, model.CheckOutDate);
                    model.Rooms = availableRooms.Select(r => new SelectListItem
                    {
                        Text = $"Room {r.RoomNumber} ({r.Type}) - {r.PricePerNight:C}",
                        Value = r.Id.ToString()
                    });
                }
            }

            return View(model);
        }

        // GET: Reservations/Edit/5
        [Authorize(Roles = "Employee, Guest")]
        public async Task<IActionResult> Edit(int? id, string source)
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

            bool isEditable = (reservation.Status == ReservationStatus.Confirmed ||
                       reservation.Status == ReservationStatus.CheckedIn);

            ViewBag.IsEditable = isEditable;
            ViewBag.Source = source;

            if (!isEditable)
            {
                ModelState.AddModelError(string.Empty, $"Cannot edit a reservation with status '{reservation.Status.GetDisplayName()}'.");
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
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> AddAmenityToReservation(int reservationId, int amenityId, int quantity, string source)
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
            return RedirectToAction(nameof(Details), new { id = reservationId, source = source });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> RemoveAmenityFromReservation(int reservationId, int reservationAmenityId, string source)
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

            return RedirectToAction(nameof(Details), new { id = reservationId, source = source });
        }


        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> Schedule()
        {
            var rooms = await _roomRepo.GetAllAsync();
            // A propriedade "Text" inclui o tipo de quarto
            ViewBag.Rooms = rooms.Select(r => new {
                Id = r.Id,
                Text = $"Room {r.RoomNumber} ({r.Type})",
                Color = GetColorForRoomType(r.Type)
            }).ToList();
            return View();
        }

        private string GetColorForRoomType(RoomType type)
        {
            return type switch
            {
                RoomType.Standard => "#5D6D7E",
                RoomType.Suite => "#AF601A",
                RoomType.Luxury => "#6C3483",
                _ => "#17202A"
            };
        }

        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> LoadSchedulerData()
        {
            // 1. Obter todas as reservas com os detalhes necessários (Hóspede, Quarto)
            var reservations = await _reservationRepo.GetAllWithDetailsAsync();

            // 2. Mapear a lista de Reservas para uma lista do nosso novo ViewModel
            var schedulerData = reservations.Select(r => new SchedulerEventViewModel
            {
                Id = r.Id,
                Subject = r.ApplicationUser?.FullName ?? "Unknown Guest",
                StartTime = r.CheckInDate,
                EndTime = r.CheckOutDate,
                RoomId = r.RoomId,
                Description = $"Status: {r.Status.GetDisplayName()}<br>Guests: {r.NumberOfGuests}",
                CategoryColor = GetColorForRoomStatus(r.Status)
            }).ToList();

            // 3. Retornar os dados em formato JSON
            return Json(schedulerData);
        }

        [HttpPost]
        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> DeleteEvent([FromBody] SchedulerBatchUpdateViewModel batchData)
        {
            if (batchData?.Deleted == null || !batchData.Deleted.Any())
            {
                return BadRequest("No items to delete.");
            }

            // Numa operação de delete do popup, só vem um item.
            var eventToDelete = batchData.Deleted.First();

            if (eventToDelete == null || eventToDelete.Id == 0)
            {
                return BadRequest("Invalid event ID.");
            }

            // O ID da reserva que queremos apagar
            int reservationId = eventToDelete.Id;

            // Chamamos a nossa lógica de negócio já existente
            var success = await PerformDeleteReservation(reservationId);

            if (success)
            {
                // IMPORTANTE: O Scheduler espera receber de volta os dados que foram processados.
                // Retornamos um objeto que tem uma propriedade "deleted" com os itens que foram apagados.
                return Json(new { deleted = batchData.Deleted });
            }
            else
            {
                return StatusCode(500, "Error deleting the reservation.");
            }
        }

        private async Task<bool> PerformDeleteReservation(int id)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);
            if (reservation == null)
            {
                return false; // Falhou, reserva não encontrada
            }

            await _reservationRepo.DeleteAsync(id);

            var room = await _roomRepo.GetByIdAsync(reservation.RoomId);
            if (room != null && room.Status == RoomStatus.Reserved)
            {
                room.Status = RoomStatus.Available;
                await _roomRepo.UpdateAsync(room);
            }

            return true; // Sucesso
        }

        // Método helper privado para atribuir cores com base no STATUS da reserva
        private string GetColorForRoomStatus(ReservationStatus status)
        {
            return status switch
            {
                ReservationStatus.Confirmed => "#0d6efd",  // Azul (Bootstrap Primary)
                ReservationStatus.CheckedIn => "#198754",   // Verde (Bootstrap Success)
                ReservationStatus.CheckedOut => "#212529",  // Preto/Cinzento Escuro (Bootstrap Dark)
                ReservationStatus.Cancelled => "#dc3545",  // Vermelho (Bootstrap Danger)
                ReservationStatus.NoShow => "#ffc107",      // Amarelo (Bootstrap Warning)
                _ => "#6c757d"                              // Cinzento (Bootstrap Secondary)
            };
        }

        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> RequestChange(int id)
        {
            // 1. Validações de segurança e negócio
            var reservation = await _reservationRepo.GetByIdAsync(id);

            // A reserva existe?
            if (reservation == null)
            {
                return NotFound();
            }

            // A reserva pertence ao utilizador logado?
            if (reservation.GuestId != _userManager.GetUserId(User))
            {
                return Forbid(); // Proibido - não é a sua reserva
            }

            // A reserva ainda pode ser alterada? (a mesma lógica do botão)
            if (reservation.Status != ReservationStatus.Confirmed || reservation.CheckInDate <= DateTime.Today)
            {
                TempData["ErrorMessage"] = "This reservation can no longer be changed.";
                return RedirectToAction(nameof(MyReservations));
            }

            var allAmenities = await _amenityRepository.GetAllAsync();

            var requestHistory = await _changeRequestRepo.GetRequestsForReservationAsync(id);


            // Prepara o ViewModel para a View
            var model = new ChangeRequestViewModel
            {
                ReservationId = id,
                // Converter a lista de amenities para o formato que a dropdown precisa
                AvailableAmenities = allAmenities.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Name} - {a.Price:C}"
                }),

                ExistingRequests = requestHistory
            };

            // 3. Retorna a View com o formulário
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> RequestChange(ChangeRequestViewModel model)
        {
            // Validações de segurança para garantir que o utilizador não está a submeter um pedido para a reserva de outra pessoa
            var reservation = await _reservationRepo.GetByIdAsync(model.ReservationId);
            if (reservation == null || reservation.GuestId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            var requestParts = new List<string>();

            if (model.SelectedAmenityId.HasValue && model.SelectedAmenityId > 0)
            {
                var amenity = await _amenityRepository.GetByIdAsync(model.SelectedAmenityId.Value);
                if (amenity != null)
                {
                    // Adiciona a informação estruturada à lista, com o marcador [SERVICE]
                    requestParts.Add($"[SERVICE] Amenity: '{amenity.Name}', Quantity: {model.AmenityQuantity}");
                }
            }

            if (!string.IsNullOrWhiteSpace(model.RequestDetails))
            {
                // Adiciona o texto livre do utilizador à lista
                requestParts.Add(model.RequestDetails);
            }

            if (!requestParts.Any())
            {
                ModelState.AddModelError(string.Empty, "Please select a service or write a request in the details box.");
            }


            if (ModelState.IsValid)
            {
                var changeRequest = new ChangeRequest
                {
                    ReservationId = model.ReservationId,
                    RequestDetails = string.Join(" ||| ", requestParts),
                    RequestedOn = DateTime.UtcNow,
                    Status = RequestStatus.Pending
                };

                await _changeRequestRepo.CreateAsync(changeRequest);

                return RedirectToAction(nameof(RequestChange), new { id = model.ReservationId });
            }

            // Se a validação falhar, repopula a dropdown de amenities antes de retornar a View
            var allAmenities = await _amenityRepository.GetAllAsync();
            model.AvailableAmenities = allAmenities.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.Name} - {a.Price:C}"
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ProcessChangeRequest(int requestId, RequestStatus newStatus, string employeeNotes)
        {
            // 1. Obter o pedido de alteração da base de dados
            var request = await _changeRequestRepo.GetByIdAsync(requestId);
            if (request == null)
            {
                return NotFound();
            }

            // 2. Atualizar as propriedades do pedido
            request.Status = newStatus;
            request.ProcessedOn = DateTime.UtcNow;

            // Guardar quem processou o pedido
            request.ProcessedByUserId = _userManager.GetUserId(User);

            request.EmployeeNotes = employeeNotes;

            // 3. Salvar as alterações na base de dados
            await _changeRequestRepo.UpdateAsync(request);

            // Redireciona de volta para a mesma página de detalhes para ver o resultado
            return RedirectToAction("Details", new { id = request.ReservationId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ProcessNoShows()
        {
            // 1. Delegar a tarefa complexa para o serviço de negócio
            var result = await _reservationService.MarkPastReservationsAsNoShowAsync();

            // 2. Usar o objeto Result do serviço para mostrar uma mensagem ao utilizador
            if (result.Succeeded)
            {
                // Se a operação foi bem-sucedida, usa a mensagem de sucesso do serviço
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                // Se falhou, usa a mensagem de erro do serviço
                TempData["ErrorMessage"] = result.Error;
            }

            // 3. Redirecionar o utilizador de volta para o dashboard
            return RedirectToAction("Index", "Home");
        }

    }
}
