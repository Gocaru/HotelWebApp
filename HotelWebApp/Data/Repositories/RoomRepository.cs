using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelWebAppContext _context;

        public RoomRepository(HotelWebAppContext context)
        {
            _context = context;
        }


        public async Task<List<Room>> GetAllAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        }


        public async Task AddAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Rooms.AnyAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync()
        {
            return await _context.Rooms
                             .Where(r => r.Status == RoomStatus.Available)
                             .OrderBy(r => r.RoomNumber)    
                             .ToListAsync();
        }
    }
}
