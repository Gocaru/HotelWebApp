using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelWebApp.Controllers
{
    /// <summary>
    /// Manages the main pages of the application, such as the homepage and privacy policy.
    /// It dynamically serves different content on the Index page based on the user's role.
    /// </summary>
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

        /// <summary>
        /// Displays the main landing page, which varies depending on the user's role.
        /// - For Employees: Shows an operational dashboard with daily tasks.
        /// - For Admins: Shows a management panel with links to system configuration.
        /// - For Guests and Anonymous Users: Shows the public-facing homepage with a hero image and room search.
        /// </summary>
        /// <returns>The appropriate view for the current user's role.</returns>
        public async Task<IActionResult> Index()
        {
            // Check if the current user is an authenticated Employee
            if (User.Identity.IsAuthenticated && User.IsInRole("Employee"))
            {
                // If so, prepare and display the employee dashboard.
                var today = DateTime.Today;

                var viewModel = new HomeDashboardViewModel
                {
                    CheckInsToday = await _reservationRepository.GetReservationsForCheckInOnDateAsync(today),
                    CheckOutsToday = await _reservationRepository.GetReservationsForCheckOutOnDateAsync(today),
                    PendingChangeRequests = await _changeRequestRepo.GetPendingRequestsAsync()
                };

                // The View "Index.cshtml" contains logic to render the dashboard when this ViewModel is provided.
                return View("Index", viewModel);
            }

            // For all other users (Guests, Admins, Anonymous), the Index view will render 
            // the public homepage or the admin panel based on its own internal role checks.
            // We don't need to pass a specific model for them here.
            return View();
        }

        /// <summary>
        /// Displays the privacy policy page.
        /// </summary>
        /// <returns>The privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays a generic error page. This is used for unhandled exceptions in production.
        /// </summary>
        /// <returns>The error view with a request identifier for debugging.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
