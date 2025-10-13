using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    /// <summary>
    /// Repository for managing data operations for the Reservation entity.
    /// Implements the IReservationRepository interface.
    /// </summary>
    public class ReservationRepository : IReservationRepository
    {
        private readonly HotelWebAppContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationRepository"/> class.
        /// </summary>
        /// <param name="context">The database context injected by the application.</param>
        public ReservationRepository(HotelWebAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously retrieves all reservations, including related Guest and Room details.
        /// The reservations are ordered by check-in date in descending order.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all reservations with their details.</returns>
        public async Task<IEnumerable<Reservation>> GetAllWithDetailsAsync()
        {
            return await _context.Reservations
                .Include(r => r.ApplicationUser)
                .Include(r => r.Room)
                .Include(r => r.ReservationAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .OrderByDescending(r => r.CheckInDate)
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves a single reservation by its ID, including related Guest and Room details.
        /// </summary>
        /// <param name="id">The ID of the reservation to find.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the reservation entity with its details, or null if not found.</returns>
        public async Task<Reservation?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.ApplicationUser)
                .Include(r => r.Room)
                .Include(r => r.ReservationAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .Include(r => r.Invoice)
                .Include(r => r.Promotion)
                .Include(r => r.ActivityBookings)  
                    .ThenInclude(ab => ab.Activity) 
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// Asynchronously adds a new reservation to the database.
        /// </summary>
        /// <param name="reservation">The reservation entity to create.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task CreateAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// synchronously updates an existing reservation in the database.
        /// </summary>
        /// <param name="reservation">The reservation entity with updated values.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Asynchronously deletes a reservation from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the reservation to delete.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task DeleteAsync(int id)
        {
            var reservation = await GetByIdWithDetailsAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }

        }

        /// <summary>
        /// Asynchronously checks if a specific room is available for a given date range, preventing booking overlaps
        /// </summary>
        /// <param name="roomId">The ID of the room to check.</param>
        /// <param name="checkIn">The desired check-in date.</param>
        /// <param name="checkOut">The desired check-out date.</param>
        /// <param name="reservationIdToExclude">Optional. The ID of an existing reservation to exclude from the check (used when editing a reservation).</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the room is available; otherwise, false.</returns>
        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? reservationIdToExclude = null)
        {
            var query = _context.Reservations
                .Where(r => r.RoomId == roomId &&
                             r.Status != ReservationStatus.Cancelled &&
                             r.Status != ReservationStatus.CheckedOut &&
                             r.Status != ReservationStatus.Completed &&
                             r.CheckInDate < checkOut &&
                             r.CheckOutDate > checkIn);

            // When editing a reservation, we must exclude it from the check,
            // otherwise it would conflict with itself.
            if (reservationIdToExclude.HasValue)
            {
                query = query.Where(r => r.Id != reservationIdToExclude.Value);
            }

            // If the query finds any overlapping reservation (.AnyAsync() == true), the room is NOT available.
            // Therefore, we return the negation (!) of the result.
            return !await query.AnyAsync();
        }

        /// <summary>
        /// Asynchronously retrieves all reservations for a specific guest, including related room details.
        /// </summary>
        /// <param name="guestId">he unique identifier of the guest (ApplicationUser ID).</param>
        /// <returns>
        /// A task that represents the asynchronous operation. 
        /// The task result contains a collection of <see cref="Reservation"/> objects for the specified guest, ordered by check-in date in descending order.
        /// Returns an empty list if the guest ID is null or empty.
        /// </returns>
        public async Task<IEnumerable<Reservation>> GetReservationsByGuestIdWithDetailsAsync(string guestId)
        {
            if(string.IsNullOrEmpty(guestId))
            {
                return new List<Reservation>();
            }

            return await _context.Reservations
                .Where(r => r.GuestId == guestId)
                .Include(r => r.Room)
                .Include(r => r.Promotion)
                .OrderByDescending(r => r.CheckInDate)
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves a single reservation by its ID without loading related entities.
        /// </summary>
        /// <param name="id">The ID of the reservation to find.</param>
        /// <returns>The reservation entity, or null if not found.</returns>
        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        /// <summary>
        /// Asynchronously retrieves all reservations scheduled for check-in on a specific date.
        /// Includes related guest and room details. It filters out cancelled reservations.
        /// </summary>
        /// <param name="date">The specific date to check for check-ins.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of reservations for check-in on the given date.</returns>
        public async Task<IEnumerable<Reservation>> GetReservationsForCheckInOnDateAsync(DateTime date)
        {
            return await _context.Reservations
                .Where(r => r.CheckInDate.Date == date.Date
                    && r.Status == ReservationStatus.Confirmed)
                .Include(r => r.ApplicationUser)
                .Include(r => r.Room)
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves all reservations scheduled for check-out on a specific date.
        /// Includes related guest and room details. It filters out cancelled reservations.
        /// </summary>
        /// <param name="date">The specific date to check for check-outs.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of reservations for check-out on the given date.</returns>
        public async Task<IEnumerable<Reservation>> GetReservationsForCheckOutOnDateAsync(DateTime date)
        {
            return await _context.Reservations
                .Where(r => r.CheckOutDate.Date == date.Date && 
                                r.Status == ReservationStatus.CheckedIn)
                .Include(r => r.ApplicationUser)
                .Include(r => r.Room)
                .ToListAsync();
        }

        /// <summary>
        /// Checks if a specific room has any active reservations scheduled for the future.
        /// </summary>
        /// <param name="roomId">The ID of the room to check.</param>
        /// <returns>True if there are future reservations; otherwise, false.</returns>
        public async Task<bool> HasFutureReservationsAsync(int roomId)
        {
            var today = DateTime.Today;

            return await _context.Reservations
                .AnyAsync(r => r.RoomId == roomId &&
                               r.Status != ReservationStatus.Cancelled &&
                               r.CheckInDate >= today);
        }

        /// <summary>
        /// Asynchronously retrieves all confirmed reservations with check-in dates in the past,
        /// which should be marked as No-Show. Includes the related Room entity.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of past-due confirmed reservations.</returns>
        public async Task<IEnumerable<Reservation>> GetPastConfirmedReservationsAsync()
        {
            var today = DateTime.Today;

            return await _context.Reservations
                .Include(r => r.Room)
                .Where(r => r.Status == ReservationStatus.Confirmed && r.CheckInDate.Date < today)
                .ToListAsync();
        }

        public async Task CancelActivityBookingsForReservationAsync(int reservationId)
        {
            var activityBookings = await _context.ActivityBookings
                .Where(ab => ab.ReservationId == reservationId
                    && (ab.Status == ActivityBookingStatus.Pending || ab.Status == ActivityBookingStatus.Confirmed))
                .ToListAsync();

            foreach (var booking in activityBookings)
            {
                booking.Status = ActivityBookingStatus.Cancelled;
            }

            if (activityBookings.Any())
            {
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"✅ Cancelled {activityBookings.Count} activity bookings for reservation {reservationId}");
            }
        }

    }
}
