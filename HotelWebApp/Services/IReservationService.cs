using HotelWebApp.Models;

namespace HotelWebApp.Services
{
    public interface IReservationService
    {
        Task<Result> CreateReservationAsync(ReservationViewModel model, string guestId);
        Task<Result> UpdateReservationAsync(int reservationId, ReservationViewModel model);
        Task<Result> CheckInReservationAsync(int reservationId);
        Task<Result> CheckOutReservationAsync(int reservationId);
        Task<Result> CancelReservationByGuestAsync(int reservationId, string guestId);
        Task<Result> CancelReservationByEmployeeAsync(int reservationId);
    }
}
