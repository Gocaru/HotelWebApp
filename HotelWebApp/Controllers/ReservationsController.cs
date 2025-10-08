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
    /// <summary>
    /// Manages all operations related to reservations, including creation, editing,
    /// and status changes like check-in/check-out.
    /// This controller serves both Employees (management) and Guests (self-service).
    /// </summary>
    public class ReservationsController : Controller
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReservationService _reservationService;
        private readonly IAmenityRepository _amenityRepository;
        private readonly IPaymentService _paymentService;
        private readonly IChangeRequestRepository _changeRequestRepo;
        private readonly IPromotionRepository _promotionRepo;

        public ReservationsController(IReservationRepository reservationRepo,
                                        IRoomRepository roomRepo,
                                        UserManager<ApplicationUser> userManager,
                                        IReservationService reservationService,
                                        IAmenityRepository amenityRepository,
                                        IPaymentService paymentService,
                                        IChangeRequestRepository changeRequestRepo,
                                        IPromotionRepository promotionRepo)
        {
            _reservationRepo = reservationRepo;
            _roomRepo = roomRepo;
            _userManager = userManager;
            _reservationService = reservationService;
            _amenityRepository = amenityRepository;
            _paymentService = paymentService;
            _changeRequestRepo = changeRequestRepo;
            _promotionRepo = promotionRepo;
        }

        // GET: Reservations
        /// <summary>
        /// Displays the main reservation management view for employees,
        /// featuring a Data Grid with all reservations.
        /// </summary>
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationRepo.GetAllWithDetailsAsync(); // Inclui User, Room e amenities

            // Map domain entities to a ViewModel tailored for the Data Grid presentation.
            // This includes calculating total cost and preparing properties for conditional UI.
            var viewModelList = reservations.Select(res =>
            {
                int numberOfNights = (res.CheckOutDate.Date - res.CheckInDate.Date).Days;
                if (numberOfNights <= 0)
                {
                    numberOfNights = 1;
                }

                decimal amenitiesCost = res.ReservationAmenities?.Sum(ra => (ra.Amenity?.Price ?? 0) * ra.Quantity) ?? 0;

                return new ReservationListViewModel
                {
                    Id = res.Id,
                    GuestName = res.ApplicationUser?.FullName ?? "N/A",
                    RoomNumber = res.Room?.RoomNumber ?? "N/A",
                    CheckInDate = res.CheckInDate,
                    CheckOutDate = res.CheckOutDate,
                    StatusText = res.Status.ToString(),
                    StatusBadgeClass = GetBadgeClassForStatus(res.Status),
                    IsOverdue = (res.Status == ReservationStatus.CheckedIn && res.CheckOutDate.Date < DateTime.Today.Date),
                    NumberOfGuests = res.NumberOfGuests,
                    RoomDetails = $"{res.Room?.RoomNumber} ({res.Room?.Type})",
                    GuestDetails = $"{res.ApplicationUser?.FullName ?? "N/A"} ({res.NumberOfGuests} pax)",
                    TotalCost = res.TotalPrice + amenitiesCost,
                    CanCheckIn = (res.Status == ReservationStatus.Confirmed && res.CheckInDate.Date <= DateTime.Today.Date),
                    CanCheckOut = (res.Status == ReservationStatus.CheckedIn),
                    CanEdit = (res.Status == ReservationStatus.Confirmed || res.Status == ReservationStatus.CheckedIn),
                    CanDelete = (res.Status == ReservationStatus.Confirmed)
                };
            }).OrderBy(r => r.CheckInDate).ToList();

            return View(viewModelList);
        }

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
        /// <summary>
        /// Displays detailed information about a single reservation.
        /// The view adapts based on whether the user is an Employee or a Guest.
        /// </summary>
        /// <param name="id">The ID of the reservation.</param>
        /// <param name="source">An optional parameter to track the navigation origin (e.g., "dashboard", "schedule") for intelligent "Back" buttons.</param>
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
                    return Forbid();
                }
            }

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
        /// <summary>
        /// Displays the form for creating a new reservation.
        /// This action supports multiple workflows:
        /// 1. A two-step process where the user first selects dates to see available rooms.
        /// 2. A direct flow where a room is pre-selected (e.g., coming from the /Rooms page).
        /// </summary>
        [Authorize(Roles = "Employee, Guest")]
        public async Task<IActionResult> Create(int? roomId, DateTime? checkInDate, DateTime? checkOutDate, string source)
        {
            var model = new ReservationViewModel();

            if (User.IsInRole("Employee"))
            {
                model.Guests = await GetGuestListItems();
            }

            if (checkInDate.HasValue && checkOutDate.HasValue)
            {
                ViewBag.TriedSearch = true;

                if (checkOutDate.Value <= checkInDate.Value)
                {
                    ModelState.AddModelError("CheckOutDate", "Check-out date must be after the check-in date.");
                    model.CheckInDate = checkInDate.Value;
                    model.CheckOutDate = checkOutDate.Value;
                    return View(model);
                }

                var availableRooms = await _roomRepo.GetAvailableRoomsAsync(checkInDate.Value, checkOutDate.Value);
                model.Rooms = availableRooms.Select(r => new SelectListItem
                {
                    Text = $"Room {r.RoomNumber} ({r.Type}) - {r.PricePerNight:C}",
                    Value = r.Id.ToString()
                });

                model.CheckInDate = checkInDate.Value;
                model.CheckOutDate = checkOutDate.Value;

                var validPromotions = await _promotionRepo.GetPromotionsForDateRangeAsync(checkInDate.Value, checkOutDate.Value);

                model.Promotions = validPromotions.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Title} (-{p.DiscountPercentage}%)"
                });
            }
            else if (roomId.HasValue)
            {
                ViewBag.TriedSearch = true;
                model.RoomId = roomId.Value;
                var selectedRoom = await _roomRepo.GetByIdAsync(roomId.Value);
                if (selectedRoom != null)
                {
                    model.Rooms = new List<SelectListItem>
            {
                new SelectListItem { Text = $"Room {selectedRoom.RoomNumber} ({selectedRoom.Type}) - {selectedRoom.PricePerNight:C}", Value = selectedRoom.Id.ToString() }
            };
                }
                model.CheckInDate = DateTime.Today.AddDays(1);
                model.CheckOutDate = DateTime.Today.AddDays(2);

                var validPromotions = await _promotionRepo.GetPromotionsForDateRangeAsync(model.CheckInDate, model.CheckOutDate);
                model.Promotions = validPromotions.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Title} (-{p.DiscountPercentage}%)"
                });
            }
            else
            {
                model.CheckInDate = DateTime.Today.AddDays(1);
                model.CheckOutDate = DateTime.Today.AddDays(2);
            }

            ViewBag.Source = source;

            return View(model);
        }


        // POST: Reservations/Create
        /// <summary>
        /// Handles the submission of the new reservation form.
        /// Validates the submitted data and uses the ReservationService to create the booking.
        /// </summary>
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

                if (model.RoomId > 0 && (model.Rooms == null || !model.Rooms.Any()))
                {
                }
                else if (User.IsInRole("Employee"))
                {
                    model.Rooms = await GetRoomListItems();
                }
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

            if (User.IsInRole("Employee"))
            {
                model.Rooms = await GetRoomListItems();
                model.Guests = await GetGuestListItems();
            }
            else 
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
        /// <summary>
        /// Handles the submission of the reservation edit form.
        /// </summary>
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

            var allGuestsList = await GetGuestListItems();
            var allRoomsList = await GetRoomListItems();

            var selectedGuests = allGuestsList.Select(g =>
            {
                g.Selected = (g.Value == reservation.GuestId);
                return g;
            });

            var selectedRooms = allRoomsList.Select(r =>
            {
                r.Selected = (r.Value == reservation.RoomId.ToString());
                return r;
            });

            var model = new ReservationViewModel
            {
                Id = reservation.Id,
                GuestId = reservation.GuestId,
                RoomId = reservation.RoomId,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                Status = reservation.Status,
                NumberOfGuests = reservation.NumberOfGuests,
                Guests = selectedGuests, 
                Rooms = selectedRooms    
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
                    ModelState.AddModelError(string.Empty, result.Error);
                }
            }

            model.Guests = await GetGuestListItems();
            model.Rooms = await GetRoomListItems();
            return View(model);
        }

        // GET: Reservations/Delete/5
        /// <summary>
        /// Displays the confirmation page before deleting a reservation.
        /// </summary>
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
        /// <summary>
        /// Deletes the specified reservation after confirmation.
        /// </summary>
        [Authorize(Roles = "Employee")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);

            if (reservation == null) return RedirectToAction(nameof(Index));

            await _reservationRepo.DeleteAsync(id);

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

        /// <summary>
        /// A private helper method for custom model validation before creating or updating a reservation.
        /// Checks for business rules like date consistency and room availability/capacity.
        /// </summary>
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

        /// <summary>
        /// Displays a list of all reservations belonging to the currently logged-in guest.
        /// </summary>
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> MyReservations()
        {
            var userId = _userManager.GetUserId(User);

            var reservations = await _reservationRepo.GetReservationsByGuestIdWithDetailsAsync(userId);

            return View(reservations);
        }

        /// <summary>
        /// Displays the confirmation page for a guest to cancel their own reservation.
        /// </summary>
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id.Value);
            if (reservation == null) return NotFound();

            if (reservation.GuestId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            if (reservation.CheckInDate <= DateTime.Today || reservation.Status != ReservationStatus.Confirmed)
            {
                TempData["ErrorMessage"] = "This reservation can no longer be cancelled.";
                return RedirectToAction(nameof(MyReservations));
            }

            return View(reservation);
        }

        /// <summary>
        /// Processes the cancellation of a reservation by a guest.
        /// </summary>
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var reservation = await _reservationRepo.GetByIdAsync(id);
            if (reservation == null) return NotFound();

            if (reservation.GuestId != _userManager.GetUserId(User) || reservation.CheckInDate <= DateTime.Today)
            {
                return Forbid();
            }

            reservation.Status = ReservationStatus.Cancelled;
            await _reservationRepo.UpdateAsync(reservation);

            var room = await _roomRepo.GetByIdAsync(reservation.RoomId);
            if (room != null)
            {
                room.Status = RoomStatus.Available;
                await _roomRepo.UpdateAsync(room);
            }

            TempData["SuccessMessage"] = "Reservation cancelled successfully.";
            return RedirectToAction(nameof(MyReservations));
        }

        /// <summary>
        /// Handles the check-in process for a reservation, updating its status and the room's status.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CheckIn(int id, string source = null)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

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

            reservation.Status = ReservationStatus.CheckedIn;
            if (reservation.Room != null)
            {
                reservation.Room.Status = RoomStatus.Occupied;
            }
            else
            {
                TempData["ErrorMessage"] = "Error: The reservation does not have an associated room.";
                return source == "dashboard" ? RedirectToAction("Index", "Home") : RedirectToAction(nameof(Index));
            }

            await _reservationRepo.UpdateAsync(reservation);

            TempData["SuccessMessage"] = $"Check-in for guest {reservation.ApplicationUser.FullName} completed successfully!";
            return RedirectToAction(nameof(Index));

            if (source == "dashboard")
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Processes the check-out for a reservation and redirects to the invoice page.
        /// </summary>
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

            if (reservation.Status != ReservationStatus.CheckedIn)
            {
                TempData["ErrorMessage"] = "This action is only available for currently checked-in reservations.";
                return source == "dashboard" ? RedirectToAction("Index", "Home") : RedirectToAction(nameof(Index));
            }

            var invoiceResult = await _paymentService.CreateInvoiceForReservationAsync(id);

            if (!invoiceResult.Succeeded)
            {
                TempData["ErrorMessage"] = invoiceResult.Error;
                return source == "dashboard" ? RedirectToAction("Index", "Home") : RedirectToAction(nameof(Index));
            }

            reservation.Status = ReservationStatus.CheckedOut;
            if (reservation.Room != null)
            {
                reservation.Room.Status = RoomStatus.Available;
            }

            await _reservationRepo.UpdateAsync(reservation);


            TempData["SuccessMessage"] = $"Check-out for guest '{reservation.ApplicationUser.FullName}' completed. Please process the payment.";

            return RedirectToAction("Details", "Invoices", new { id = invoiceResult.Data.Id });
        }

        /// <summary>
        /// Adds an extra service (amenity) to an existing reservation.
        /// </summary>
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
                TempData["ErrorMessage"] = result.Error;
            }
            else
            {
                TempData["SuccessMessage"] = "Amenity added successfully!";
            }

            return RedirectToAction(nameof(Details), new { id = reservationId, source = source });
        }

        /// <summary>
        /// Removes an extra service (amenity) from an existing reservation.
        /// </summary>
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

        /// <summary>
        /// Displays the reservation schedule/calendar view.
        /// </summary>
        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> Schedule()
        {
            var rooms = await _roomRepo.GetAllAsync();
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

        /// <summary>
        /// API-like endpoint that provides reservation data in JSON format for the Scheduler component.
        /// </summary>
        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> LoadSchedulerData()
        {
            var reservations = await _reservationRepo.GetAllWithDetailsAsync();

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

            return Json(schedulerData);
        }

        /// <summary>
        /// API-like endpoint called by the Scheduler to delete an event (reservation).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> DeleteEvent([FromBody] SchedulerBatchUpdateViewModel batchData)
        {
            if (batchData?.Deleted == null || !batchData.Deleted.Any())
            {
                return BadRequest("No items to delete.");
            }

            var eventToDelete = batchData.Deleted.First();

            if (eventToDelete == null || eventToDelete.Id == 0)
            {
                return BadRequest("Invalid event ID.");
            }

            int reservationId = eventToDelete.Id;

            var success = await PerformDeleteReservation(reservationId);

            if (success)
            {
                return Json(new { deleted = batchData.Deleted });
            }
            else
            {
                return StatusCode(500, "Error deleting the reservation.");
            }
        }

        /// <summary>
        /// A private helper method to perform the actual deletion of a reservation
        /// and update the associated room's status.
        /// This logic is centralized here to be reused by different delete actions (e.g., from the grid and the schedule).
        /// </summary>
        private async Task<bool> PerformDeleteReservation(int id)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);
            if (reservation == null)
            {
                return false;
            }

            await _reservationRepo.DeleteAsync(id);

            var room = await _roomRepo.GetByIdAsync(reservation.RoomId);
            if (room != null && room.Status == RoomStatus.Reserved)
            {
                room.Status = RoomStatus.Available;
                await _roomRepo.UpdateAsync(room);
            }

            return true; 
        }


        private string GetColorForRoomStatus(ReservationStatus status)
        {
            return status switch
            {
                ReservationStatus.Confirmed => "#0d6efd",  // Blue (Bootstrap Primary)
                ReservationStatus.CheckedIn => "#198754",   // Green (Bootstrap Success)
                ReservationStatus.CheckedOut => "#212529",  // Black/Dark Gray (Bootstrap Dark)
                ReservationStatus.Cancelled => "#dc3545",  // Red (Bootstrap Danger)
                ReservationStatus.NoShow => "#ffc107",      // Yellow (Bootstrap Warning)
                _ => "#6c757d"                              // Gray (Bootstrap Secondary)
            };
        }

        /// <summary>
        /// Displays the form for a guest to request a change to their reservation.
        /// </summary>
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> RequestChange(int id)
        {
            var reservation = await _reservationRepo.GetByIdAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.GuestId != _userManager.GetUserId(User))
            {
                return Forbid(); 
            }

            if (reservation.Status != ReservationStatus.Confirmed || reservation.CheckInDate <= DateTime.Today)
            {
                TempData["ErrorMessage"] = "This reservation can no longer be changed.";
                return RedirectToAction(nameof(MyReservations));
            }

            var allAmenities = await _amenityRepository.GetAllAsync();

            var requestHistory = await _changeRequestRepo.GetRequestsForReservationAsync(id);


            var model = new ChangeRequestViewModel
            {
                ReservationId = id,
                AvailableAmenities = allAmenities.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Name} - {a.Price:C}"
                }),

                ExistingRequests = requestHistory
            };

            return View(model);
        }

        /// <summary>
        /// Handles the submission of a change request from a guest.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> RequestChange(ChangeRequestViewModel model)
        {
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
                    requestParts.Add($"[SERVICE] Amenity: '{amenity.Name}', Quantity: {model.AmenityQuantity}");
                }
            }

            if (!string.IsNullOrWhiteSpace(model.RequestDetails))
            {
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

            var allAmenities = await _amenityRepository.GetAllAsync();
            model.AvailableAmenities = allAmenities.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.Name} - {a.Price:C}"
            });

            return View(model);
        }

        /// <summary>
        /// Handles the processing (approval or rejection) of a change request by an employee.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ProcessChangeRequest(int requestId, RequestStatus newStatus, string employeeNotes)
        {
            var request = await _changeRequestRepo.GetByIdAsync(requestId);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = newStatus;
            request.ProcessedOn = DateTime.UtcNow;

            request.ProcessedByUserId = _userManager.GetUserId(User);

            request.EmployeeNotes = employeeNotes;

            await _changeRequestRepo.UpdateAsync(request);

            return RedirectToAction("Details", new { id = request.ReservationId });
        }

        /// <summary>
        /// Triggers the service to process all past-due reservations and mark them as 'No-Show'.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ProcessNoShows()
        {
            var result = await _reservationService.MarkPastReservationsAsNoShowAsync();

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
