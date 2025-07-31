using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelWebApp.Controllers
{
    /// <summary>
    /// Manages the display and administration of hotel rooms.
    /// Public actions allow anonymous users to view rooms, while protected actions are for Admins.
    /// </summary>
    public class RoomsController : Controller
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RoomsController(IRoomRepository roomRepository, 
                               IReservationRepository reservationRepository, 
                               IWebHostEnvironment webHostEnvironment)
        {
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: RoomsController
        /// <summary>
        /// Displays a public list of all hotel rooms.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return View(rooms);
        }

        // GET: RoomsController/Details/5
        /// <summary>
        /// Displays the public details of a specific room.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // GET: RoomsController/Create
        /// <summary>
        /// Displays the form for an Admin to create a new room.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var viewModel = new RoomViewModel();
            PopulateDropdowns();
            return View(viewModel);
        }

        // POST: RoomsController/Create
        /// <summary>
        /// Handles the creation of a new room, including the image upload.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RoomViewModel viewModel)
        {
            if (await _roomRepository.RoomNumberExistsAsync(viewModel.RoomNumber))
            {
                ModelState.AddModelError("RoomNumber", "A room with this number already exists.");
            }

            if (ModelState.IsValid)
            {
                // Process and save the uploaded image file, getting its unique name.
                string uniqueFileName = await ProcessUploadedFile(viewModel);

                var room = new Room
                {
                    RoomNumber = viewModel.RoomNumber,
                    Capacity = viewModel.Capacity,
                    PricePerNight = viewModel.PricePerNight,
                    Type = viewModel.Type.Value,
                    Status = viewModel.Status.Value,
                    ImageUrl = uniqueFileName 
                };

                await _roomRepository.AddAsync(room);
                TempData["SuccessMessage"] = "Room created successfully.";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns();
            return View(viewModel);
        }

        // GET: RoomsController/Edit/5
        /// <summary>
        /// Displays the form for an Admin to edit an existing room.
        /// Prevents editing if the room has active or future reservations.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var roomToEdit = await _roomRepository.GetByIdAsync(id);

            if (roomToEdit == null)
            {
                return NotFound();
            }

            bool hasActiveOrFutureReservations = await _reservationRepository.HasFutureReservationsAsync(id);

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
                Status = roomToEdit.Status,
                ImageUrl = roomToEdit.ImageUrl
            };

            PopulateDropdowns();
            return View(viewModel);
        }

        // POST: RoomsController/Edit/5
        /// <summary>
        /// Handles the update of a room's details, including replacing the image if a new one is provided.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, RoomViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (await _roomRepository.RoomNumberExistsAsync(viewModel.RoomNumber, viewModel.Id))
            {
                ModelState.AddModelError("RoomNumber", "Another room with this number already exists.");
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(viewModel);
            }

            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();

            if (viewModel.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(room.ImageUrl))
                {
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/rooms", room.ImageUrl);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                room.ImageUrl = await ProcessUploadedFile(viewModel);
            }

            room.RoomNumber = viewModel.RoomNumber;
            room.Capacity = viewModel.Capacity;
            room.PricePerNight = viewModel.PricePerNight;
            room.Type = viewModel.Type.Value;
            room.Status = viewModel.Status.Value;

            await _roomRepository.UpdateAsync(room);
            TempData["SuccessMessage"] = "Room updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: RoomsController/Delete/5
        /// <summary>
        /// Displays the confirmation page before deleting a room.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // POST: RoomsController/Delete/5
        /// <summary>
        /// Deletes a room after confirmation.
        /// Prevents deletion if the room has any future reservations.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Business rule: A room with scheduled future bookings cannot be deleted.
            bool hasFutureReservations = await _reservationRepository.HasFutureReservationsAsync(id);

            if (hasFutureReservations)
            {
                var room = await _roomRepository.GetByIdAsync(id);

                TempData["ErrorMessage"] = $"The Room '{room?.RoomNumber}' cannot be deleted because it has future reservations scheduled.";

                return RedirectToAction(nameof(Index));
            }

            await _roomRepository.DeleteAsync(id);

            TempData["SuccessMessage"] = "Room deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// A private helper method to populate dropdown lists for the Create/Edit views.
        /// </summary>
        [Authorize(Roles = "Admin")]
        private void PopulateDropdowns()
        {
            ViewBag.RoomTypes = Enum.GetValues(typeof(RoomType));
            ViewBag.RoomStatuses = Enum.GetValues(typeof(RoomStatus));
        }

        /// <summary>
        /// Displays a list of rooms that are available for a specific date range,
        /// based on a search from the homepage.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> SearchResults(DateTime checkInDate, DateTime checkOutDate)
        {
            if (checkOutDate <= checkInDate)
            {
                TempData["ErrorMessage"] = "Check-out date must be after the check-in date.";
                return RedirectToAction("Index", "Home");
            }

            var availableRooms = await _roomRepository.GetAvailableRoomsAsync(checkInDate, checkOutDate);

            ViewBag.CheckInDate = checkInDate;
            ViewBag.CheckOutDate = checkOutDate;

            return View(availableRooms);
        }

        /// <summary>
        /// A private helper method to process and save an uploaded room image.
        /// </summary>
        /// <param name="viewModel">The view model containing the IFormFile.</param>
        /// <returns>The unique file name of the saved image, or null if no file was uploaded.</returns>
        private async Task<string> ProcessUploadedFile(RoomViewModel viewModel)
        {
            string uniqueFileName = null;

            if (viewModel.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/rooms");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.ImageFile.CopyToAsync(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
