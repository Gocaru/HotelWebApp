using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HotelWebApp.Data.Entities;

namespace HotelWebApp.Controllers
{
    /// <summary>
    /// API controller to expose reservation history data.
    /// This API is public and does not require authentication.
    /// </summary>
    [Route("api/[controller]")] // Sets the base route to /api/HistoryApi
    [AllowAnonymous]
    public class HistoryApiController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public HistoryApiController(IReservationRepository reservationRepo, 
                                    UserManager<ApplicationUser> userManager)
        {
            _reservationRepo = reservationRepo;
            _userManager = userManager;
        }

        // GET: /api/HistoryApi/ReservationsByGuest/{guestId}
        /// <summary>
        /// Retrieves the complete reservation history for a specific guest.
        /// </summary>
        /// <param name="guestId">The unique identifier (GUID) of the guest.</param>
        /// <returns>
        /// An ActionResult containing a list of reservations in JSON format.
        /// Returns 200 OK with data on success.
        /// Returns 400 Bad Request if the guestId is missing.
        /// Returns 404 Not Found if the user does not exist, is not a guest, or has no reservations.
        /// </returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/HistoryApi/ReservationsByGuest/c4802ac5-4676-4a7f-8688-a14a1acc389d
        ///
        /// </remarks>
        [HttpGet("ReservationsByGuest/{guestId}")]
        public async Task<ActionResult<IEnumerable<ReservationApiViewModel>>> GetReservationsByGuestId(string guestId)
        {
            if (string.IsNullOrEmpty(guestId))
            {
                return BadRequest("Guest ID is required.");
            }

            var user = await _userManager.FindByIdAsync(guestId);
            if (user == null)
            {
                return NotFound($"No guest found with ID '{guestId}'.");
            }

            // Business rule: Only users with the 'Guest' role can have reservations.
            var isGuest = await _userManager.IsInRoleAsync(user, "Guest");
            if (!isGuest)
            {
                return NotFound($"The user with ID '{guestId}' is not a guest and has no reservations.");
            }

            var reservations = await _reservationRepo.GetReservationsByGuestIdWithDetailsAsync(guestId);

            if (reservations == null || !reservations.Any())
            {
                return NotFound($"No reservations found for guest with ID '{guestId}'.");
            }

            // Map the domain entities to a public-facing DTO (Data Transfer Object)
            var result = reservations.Select(res =>
            {
                // Calculate total cost for the reservation
                int numberOfNights = (res.CheckOutDate.Date - res.CheckInDate.Date).Days;
                if (numberOfNights <= 0) numberOfNights = 1;
                decimal stayCost = numberOfNights * (res.Room?.PricePerNight ?? 0);
                decimal amenitiesCost = res.ReservationAmenities?.Sum(ra => (ra.Amenity?.Price ?? 0) * ra.Quantity) ?? 0;

                return new ReservationApiViewModel
                {
                    ReservationId = res.Id,
                    RoomNumber = res.Room?.RoomNumber ?? "N/A",
                    RoomType = res.Room?.Type.ToString() ?? "N/A",
                    CheckInDate = res.CheckInDate,
                    CheckOutDate = res.CheckOutDate,
                    TotalCost = stayCost + amenitiesCost,
                    Status = res.Status.ToString()
                };
            });

            return Ok(result);
        }
    }
}
