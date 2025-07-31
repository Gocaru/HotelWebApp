using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    /// <summary>
    /// Defines the contract for the change request repository, specifying the data access operations
    /// available for the ChangeRequest entity.
    /// </summary>
    public interface IChangeRequestRepository
    {
        /// <summary>
        /// Asynchronously adds a new change request to the data store.
        /// </summary>
        /// <param name="changeRequest">The ChangeRequest entity to create.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task CreateAsync(ChangeRequest changeRequest);

        /// <summary>
        /// Asynchronously retrieves all change requests that are currently in 'Pending' status across all reservations.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of pending change requests.</returns>
        Task<IEnumerable<ChangeRequest>> GetPendingRequestsAsync();

        /// <summary>
        /// Asynchronously retrieves the most recent pending change request for a single, specific reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to check for pending requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the pending ChangeRequest entity, or null if none is found.</returns>
        Task<ChangeRequest?> GetPendingRequestForReservationAsync(int reservationId);

        /// <summary>
        /// Asynchronously retrieves a single change request by its unique identifier.
        /// </summary>
        /// <param name="id">The primary key of the change request.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ChangeRequest entity, or null if not found.</returns>
        Task<ChangeRequest?> GetByIdAsync(int id);

        /// <summary>
        /// Asynchronously updates an existing change request in the data store.
        /// </summary>
        /// <param name="changeRequest">The ChangeRequest entity with updated values.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task UpdateAsync(ChangeRequest changeRequest);

        /// <summary>
        /// Asynchronously retrieves the complete history of all change requests (pending, approved, rejected) for a specific reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all change requests for the specified reservation.</returns>
        Task<IEnumerable<ChangeRequest>> GetRequestsForReservationAsync(int reservationId);
    }
}
