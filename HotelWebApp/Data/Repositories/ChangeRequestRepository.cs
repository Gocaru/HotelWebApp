using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    public class ChangeRequestRepository : IChangeRequestRepository
    {
        private readonly HotelWebAppContext _context;

        public ChangeRequestRepository(HotelWebAppContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(ChangeRequest changeRequest)
        {
            _context.ChangeRequests.Add(changeRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChangeRequest>> GetPendingRequestsAsync()
        {
            // Obtém todos os pedidos pendentes e inclui os detalhes da reserva associada
            return await _context.ChangeRequests
                                 .Include(cr => cr.Reservation)
                                    .ThenInclude(r => r.ApplicationUser) // Para sabermos o nome do hóspede
                                 .Where(cr => cr.Status == RequestStatus.Pending)
                                 .OrderBy(cr => cr.RequestedOn)
                                 .ToListAsync();
        }

        public async Task<ChangeRequest?> GetPendingRequestForReservationAsync(int reservationId)
        {
            return await _context.ChangeRequests
                .FirstOrDefaultAsync(cr => cr.ReservationId == reservationId && cr.Status == RequestStatus.Pending);
        }

        public async Task<ChangeRequest?> GetByIdAsync(int id)
        {
            return await _context.ChangeRequests.FindAsync(id);
        }

        public async Task UpdateAsync(ChangeRequest changeRequest)
        {
            _context.ChangeRequests.Update(changeRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChangeRequest>> GetRequestsForReservationAsync(int reservationId)
        {
            return await _context.ChangeRequests
                                 .Where(r => r.ReservationId == reservationId)
                                 .OrderByDescending(r => r.RequestedOn)
                                 .ToListAsync();
        }
    }
}
