using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly HotelWebAppContext _context;   

        public AmenityRepository(HotelWebAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Amenity>> GetAllAsync()
        {
            return await _context.Amenities.OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<Amenity?> GetByIdAsync(int id)
        {
            return await _context.Amenities.FindAsync(id); ;
        }

        public async Task CreateAsync(Amenity amenity)
        {
            await _context.Amenities.AddAsync(amenity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Amenity amenity)
        {
            // Marca a entidade como modificada.
            _context.Amenities.Update(amenity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Amenity amenity)
        {
            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AmenityExistsAsync(int id)
        {
            return await _context.Amenities.AnyAsync(e => e.Id == id);
        }

    }
}
