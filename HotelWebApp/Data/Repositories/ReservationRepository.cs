using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly HotelWebAppContext _context;

        public ReservationRepository(HotelWebAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                                 .Include(r => r.Guest)
                                 .Include(r => r.Room)
                                 .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations
                                 .Include(r => r.Guest)
                                 .Include(r => r.Room)
                                 .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task CreateAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }

    }
}
