using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    /// <summary>
    /// Defines the contract for the reservation repository, specifying data access operations
    /// for the Reservation entity.
    /// </summary>
    public interface IReservationRepository
    {
        /// <summary>
        /// Asynchronously retrieves all reservations from the data store, eagerly loading related entities
        /// like Guest, Room, and Amenities for a comprehensive view.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all reservations with their details.</returns>
        Task<IEnumerable<Reservation>> GetAllWithDetailsAsync();

        /// <summary>
        /// Asynchronously retrieves a single reservation by its ID, eagerly loading all related entities.
        /// </summary>
        /// <param name="id">The primary key of the reservation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the detailed Reservation entity, or null if not found.</returns>
        Task<Reservation?> GetByIdWithDetailsAsync (int id);

        /// <summary>
        /// Asynchronously adds a new reservation to the data store.
        /// </summary>
        /// <param name="reservation">The Reservation entity to create.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task CreateAsync(Reservation reservation);

        /// <summary>
        /// Asynchronously updates an existing reservation in the data store.
        /// </summary>
        /// <param name="reservation">The Reservation entity with updated values.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task UpdateAsync(Reservation reservation);

        /// <summary>
        /// Asynchronously deletes a reservation from the data store by its ID.
        /// </summary>
        /// <param name="id">The ID of the reservation to delete.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Asynchronously checks if a specific room is available for booking within a given date range.
        /// It detects booking overlaps with existing confirmed or checked-in reservations.
        /// </summary>
        /// <param name="roomId">The ID of the room to check.</param>
        /// <param name="checkIn">The desired check-in date.</param>
        /// <param name="checkOut">The desired check-out date.</param>
        /// <param name="reservationIdToExclude">Optional. An existing reservation ID to exclude from the check, used when editing a reservation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the room is available; otherwise, false.</returns>
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? reservationIdToExclude = null);

        /// <summary>
        /// Asynchronously retrieves all reservations made by a specific guest.
        /// </summary>
        /// <param name="guestId">The unique identifier of the guest (ApplicationUser ID).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of reservations for the specified guest.</returns>
        Task<IEnumerable<Reservation>> GetReservationsByGuestIdWithDetailsAsync(string guestId);


        /// <summary>
        /// Asynchronously retrieves a single reservation by its ID without loading related entities.
        /// </summary>
        /// <param name="id">The ID of the reservation to find.</param>
        /// <returns>The basic Reservation entity, or null if not found.</returns>
        Task<Reservation?> GetByIdAsync(int id);

        /// <summary>
        /// Asynchronously retrieves all active reservations scheduled for check-in on a specific date.
        /// </summary>
        /// <param name="date">The date to check for check-ins.</param>
        /// <returns>A collection of reservations due for check-in.</returns>
        Task<IEnumerable<Reservation>> GetReservationsForCheckInOnDateAsync(DateTime date);

        /// <summary>
        /// Asynchronously retrieves all active reservations scheduled for check-out on a specific date.
        /// </summary>
        /// <param name="date">The date to check for check-outs.</param>
        /// <returns>A collection of reservations due for check-out.</returns>
        Task<IEnumerable<Reservation>> GetReservationsForCheckOutOnDateAsync(DateTime date);

        /// <summary>
        /// Asynchronously checks if a specific room has any active or confirmed reservations scheduled for the future.
        /// Used to prevent editing or deleting a room that is already booked.
        /// </summary>
        /// <param name="roomId">The ID of the room to check.</param>
        /// <returns>True if there are future reservations; otherwise, false.</returns>
        Task<bool> HasFutureReservationsAsync(int roomId);

        /// <summary>
        /// Asynchronously retrieves all confirmed reservations with check-in dates in the past,
        /// which should be marked as No-Show. Includes the related Room entity.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of past-due confirmed reservations.</returns>
        Task<IEnumerable<Reservation>> GetPastConfirmedReservationsAsync();

    }
}
