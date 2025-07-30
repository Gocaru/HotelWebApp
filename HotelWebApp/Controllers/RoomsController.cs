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
                string uniqueFileName = await ProcessUploadedFile(viewModel);

                var room = new Room
                {
                    RoomNumber = viewModel.RoomNumber,
                    Capacity = viewModel.Capacity,
                    PricePerNight = viewModel.PricePerNight,
                    Type = viewModel.Type.Value,
                    Status = viewModel.Status.Value,
                    ImageUrl = uniqueFileName // Atribuir o nome do ficheiro guardado
                };

                await _roomRepository.AddAsync(room);
                TempData["SuccessMessage"] = "Room created successfully.";
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
                Status = roomToEdit.Status,
                ImageUrl = roomToEdit.ImageUrl
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

            // Passamos o ModelState para a action POST Create.
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(viewModel);
            }

            // Obter o quarto existente da base de dados
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();

            // Se um novo ficheiro de imagem foi enviado, processa-o
            if (viewModel.ImageFile != null)
            {
                // Se já existia uma imagem antiga, apaga-a do disco
                if (!string.IsNullOrEmpty(room.ImageUrl))
                {
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/rooms", room.ImageUrl);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Guarda o novo ficheiro e obtém o seu nome único
                room.ImageUrl = await ProcessUploadedFile(viewModel);
            }

            // Atualiza as outras propriedades do quarto
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

        [AllowAnonymous]
        public async Task<IActionResult> SearchResults(DateTime checkInDate, DateTime checkOutDate)
        {
            if (checkOutDate <= checkInDate)
            {
                // Redirecionar de volta para a Home com uma mensagem de erro
                TempData["ErrorMessage"] = "Check-out date must be after the check-in date.";
                return RedirectToAction("Index", "Home");
            }

            // Usar o repositório para encontrar os quartos disponíveis
            var availableRooms = await _roomRepository.GetAvailableRoomsAsync(checkInDate, checkOutDate);

            // Passar as datas para a View para que elas possam ser mostradas
            ViewBag.CheckInDate = checkInDate;
            ViewBag.CheckOutDate = checkOutDate;

            return View(availableRooms);
        }

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
