using HotelWebApp.Models;

namespace HotelWebApp.Services
{
    /// <summary>
    /// Defines the contract for the core reservation business logic service.
    /// This service handles all complex operations related to the lifecycle of a reservation.
    /// </summary>
    public interface IReservationService
    {
        /// <summary>
        /// Creates a new reservation based on the provided view model data.
        /// </summary>
        /// <param name="model">The view model containing the new reservation details.</param>
        /// <param name="guestId">The ID of the guest making the reservation.</param>
        /// <returns>A Result object indicating the outcome of the creation process.</returns>
        Task<Result> CreateReservationAsync(ReservationViewModel model, string guestId);

        /// <summary>
        /// Updates an existing reservation with new details from the view model.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to update.</param>
        /// <param name="model">The view model containing the updated reservation details.</param>
        /// <returns>A Result object indicating the outcome of the update process.</returns>
        Task<Result> UpdateReservationAsync(int reservationId, ReservationViewModel model);

        /// <summary>
        /// Processes the check-in for a reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to check in.</param>
        /// <returns>A Result object indicating the outcome.</returns>
        Task<Result> CheckInReservationAsync(int reservationId);

        /// <summary>
        /// Processes the check-out for a reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to check out.</param>
        /// <returns>A Result object indicating the outcome.</returns>
        Task<Result> CheckOutReservationAsync(int reservationId);

        /// <summary>
        /// Cancels a reservation, initiated by a guest. Includes security checks.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to cancel.</param>
        /// <param name="guestId">The ID of the guest attempting the cancellation, for validation.</param>
        /// <returns>A Result object indicating the outcome.</returns>
        Task<Result> CancelReservationByGuestAsync(int reservationId, string guestId);

        /// <summary>
        /// Cancels a reservation, initiated by an employee.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to cancel.</param>
        /// <returns>A Result object indicating the outcome.</returns>
        Task<Result> CancelReservationByEmployeeAsync(int reservationId);

        /// <summary>
        /// Adds an extra service (amenity) to a reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation.</param>
        /// <param name="amenityId">The ID of the amenity to add.</param>
        /// <param name="quantity">The quantity of the amenity to add.</param>
        /// <returns>A Result object indicating the outcome.</returns>
        Task<Result> AddAmenityToReservationAsync(int reservationId, int amenityId, int quantity);

        /// <summary>
        /// Removes a previously added service from a reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation.</param>
        /// <param name="reservationAmenityId">The ID of the specific ReservationAmenity record to remove.</param>
        /// <returns>A Result object indicating the outcome.</returns>
        Task<Result> RemoveAmenityFromReservationAsync(int reservationId, int reservationAmenityId);

        /// <summary>
        /// A batch process that finds all past-due confirmed reservations and marks them as 'No-Show'.
        /// This is intended to be run periodically (e.g., daily).
        /// </summary>
        /// <returns>A Result object summarizing the outcome of the batch operation.</returns>
        Task<Result> MarkPastReservationsAsNoShowAsync();

    }
}
