using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data
{
    public class GuestRepository : IGuestRepository
    {
        private readonly DataContext _context;

        public GuestRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Guest>> GetAllAsync()
        {
            return await _context.Guests.ToListAsync();
        }

        public async Task<Guest?> GetByIdAsync(int id)
        {
            return await _context.Guests.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task AddAsync(Guest guest)
        {
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guest guest)
        {
            _context.Guests.Update(guest);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if(guest != null)
            {
                _context.Guests.Remove(guest);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Guests.AnyAsync(g => g.Id == id);
        }

    }
}
