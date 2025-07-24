using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IReservationRepository _reservationRepository;
        private readonly IChangeRequestRepository _changeRequestRepo;

        public HomeController(ILogger<HomeController> logger,
                              IReservationRepository reservationRepository, 
                              IChangeRequestRepository changeRequestRepo)
        {
            _logger = logger;
            _reservationRepository = reservationRepository;
            _changeRequestRepo = changeRequestRepo;
        }

        public async Task<IActionResult> Index()
        {
            // Verificamos se o utilizador está autenticado e se pertence ao role funcionário
            if (User.Identity.IsAuthenticated && User.IsInRole("Employee"))
            {
                // Se for funcionário, preparamos o dashboard
                var today = DateTime.Today;

                var viewModel = new HomeDashboardViewModel
                {
                    CheckInsToday = await _reservationRepository.GetReservationsForCheckInOnDateAsync(today),
                    CheckOutsToday = await _reservationRepository.GetReservationsForCheckOutOnDateAsync(today),

                    PendingChangeRequests = await _changeRequestRepo.GetPendingRequestsAsync()
                };

                // Enviamos o ViewModel para a View
                return View("Index", viewModel);
            }

            // Para todos os outros utilizadores (anónimos, clientes), mostramos a view padrão sem dados
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
