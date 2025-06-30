using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllWithDetailsAsync();

        Task<Reservation?> GetByIdWithDetailsAsync (int id);

        Task CreateAsync(Reservation reservation);

        Task UpdateAsync(Reservation reservation);

        Task DeleteAsync(int id);

        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? reservationIdToExclude = null);

    }
}
