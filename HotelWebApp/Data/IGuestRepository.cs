using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data
{
    public interface IGuestRepository
    {
        Task<List<Guest>> GetAllAsync();

        Task<Guest?> GetByIdAsync(int id);

        Task AddAsync(Guest guest);

        Task UpdateAsync(Guest guest);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
