using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    /// <summary>
    /// Implements the IRoomRepository interface to provide data access logic
    /// for the Room entity using Entity Framework Core.
    /// </summary>
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

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            var checkInDateOnly = checkIn.Date;
            var checkOutDateOnly = checkOut.Date;

            var blockingStatuses = new[]
            {
                ReservationStatus.Confirmed,
                ReservationStatus.CheckedIn
            };

            // Step 1: Find the IDs of all rooms that have conflicting reservations.
            // A conflict occurs if an existing reservation starts before the new one ends,
            // and ends after the new one starts.
            var unavailableRoomIds = await _context.Reservations
                .Where(r => blockingStatuses.Contains(r.Status) &&
                            r.CheckInDate.Date < checkOutDateOnly &&
                            r.CheckOutDate > checkIn)
                .Select(r => r.RoomId)
                .Distinct()
                .ToListAsync();

            // Step 2: Return all rooms that are NOT in maintenance and NOT in the unavailable list.
            return await _context.Rooms
                .Where(r => r.Status != RoomStatus.Maintenance &&
                             !unavailableRoomIds.Contains(r.Id))
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<bool> RoomNumberExistsAsync(string roomNumber, int? excludeId = null)
        {
            var query = _context.Rooms.Where(r => r.RoomNumber.ToLower() == roomNumber.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
