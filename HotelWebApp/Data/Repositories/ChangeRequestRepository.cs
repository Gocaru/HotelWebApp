using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    /// <summary>
    /// Repository for handling data operations related to the ChangeRequest entity.
    /// Manages guest requests to modify their reservations.
    /// </summary>
    public class ChangeRequestRepository : IChangeRequestRepository
    {
        private readonly HotelWebAppContext _context;

        public ChangeRequestRepository(HotelWebAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new change request in the database.
        /// </summary>
        /// <param name="changeRequest">The ChangeRequest entity to add.</param>
        public async Task CreateAsync(ChangeRequest changeRequest)
        {
            _context.ChangeRequests.Add(changeRequest);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all change requests that are currently in 'Pending' status.
        /// Eagerly loads related Reservation and Guest data for display purposes (e.g., on the dashboard).
        /// </summary>
        /// <returns>A collection of pending ChangeRequest entities.</returns>
        public async Task<IEnumerable<ChangeRequest>> GetPendingRequestsAsync()
        {
            return await _context.ChangeRequests
                                 .Include(cr => cr.Reservation)
                                    .ThenInclude(r => r.ApplicationUser) // Include the guest's name from the reservation
                                 .Where(cr => cr.Status == RequestStatus.Pending)
                                 .OrderBy(cr => cr.RequestedOn)
                                 .ToListAsync();
        }

        /// <summary>
        /// Finds the most recent pending change request for a specific reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to check.</param>
        /// <returns>The pending ChangeRequest entity, or null if none exists.</returns>
        public async Task<ChangeRequest?> GetPendingRequestForReservationAsync(int reservationId)
        {
            // Note: This assumes a business rule of only one pending request per reservation at a time.
            return await _context.ChangeRequests
                .FirstOrDefaultAsync(cr => cr.ReservationId == reservationId && cr.Status == RequestStatus.Pending);
        }

        /// <summary>
        /// Retrieves a single change request by its unique identifier.
        /// </summary>
        /// <param name="id">The primary key of the change request.</param>
        /// <returns>The ChangeRequest entity, or null if not found.</returns>
        public async Task<ChangeRequest?> GetByIdAsync(int id)
        {
            return await _context.ChangeRequests.FindAsync(id);
        }

        /// <summary>
        /// Updates an existing change request in the database, typically to change its status.
        /// </summary>
        /// <param name="changeRequest">The ChangeRequest entity with updated values.</param>
        public async Task UpdateAsync(ChangeRequest changeRequest)
        {
            _context.ChangeRequests.Update(changeRequest);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves the complete history of all change requests for a specific reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation.</param>
        /// <returns>A collection of all ChangeRequest entities associated with the reservation, ordered by most recent first.</returns>
        public async Task<IEnumerable<ChangeRequest>> GetRequestsForReservationAsync(int reservationId)
        {
            return await _context.ChangeRequests
                                 .Where(r => r.ReservationId == reservationId)
                                 .OrderByDescending(r => r.RequestedOn)
                                 .ToListAsync();
        }
    }
}
