using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetAllAsync();

        Task<Room?> GetByIdAsync(int id);

        Task AddAsync(Room room);

        Task UpdateAsync(Room room);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
