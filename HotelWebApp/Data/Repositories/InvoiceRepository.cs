using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly HotelWebAppContext _context;

        public InvoiceRepository(HotelWebAppContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices.FindAsync(id);
        }

        public async Task<Invoice?> GetByReservationIdAsync(int reservationId)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.ReservationId == reservationId);
        }

        public async Task CreateAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }
    }
}