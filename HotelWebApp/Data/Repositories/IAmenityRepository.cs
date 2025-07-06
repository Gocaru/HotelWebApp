using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    public interface IAmenityRepository
    {
        Task<IEnumerable<Amenity>> GetAllAsync();

        Task<Amenity?> GetByIdAsync(int id);

        Task CreateAsync(Amenity amenity);

        Task UpdateAsync(Amenity amenity);

        Task DeleteAsync(Amenity amenity);

        Task<bool> AmenityExistsAsync(int id);

        Task<bool> IsInUseAsync(int id);
    }
}
