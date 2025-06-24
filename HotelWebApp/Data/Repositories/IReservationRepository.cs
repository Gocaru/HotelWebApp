using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();

        Task<Reservation?> GetByIdAsync (int id);

        Task CreateAsync(Reservation reservation);

        Task UpdateAsync(Reservation reservation);

        Task DeleteAsync(Reservation reservation);

    }
}
