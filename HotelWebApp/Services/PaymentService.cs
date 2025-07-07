using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HotelWebAppContext _context;

        public PaymentService(HotelWebAppContext context)
        {
            _context = context;
        }

        public async Task<Result<Invoice>> CreateInvoiceForReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.ReservationAmenities)
                .Include(r => r.Invoice)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
            {
                return Result<Invoice>.Failure("Reservation not found.");
            }

            if (reservation.Invoice != null)
            {
                // A fatura já existe, vamos retorná-la em vez de criar uma nova.
                return Result<Invoice>.Success(reservation.Invoice);
            }

            // Calcula o valor final da fatura (preço das noites + custo das amenities)
            decimal amenitiesTotal = reservation.ReservationAmenities.Sum(ra => ra.PriceAtTimeOfBooking * ra.Quantity);
            decimal finalInvoiceAmount = reservation.TotalPrice + amenitiesTotal;

            var invoice = new Invoice
            {
                ReservationId = reservation.Id,
                GuestId = reservation.GuestId,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = finalInvoiceAmount,
                Status = InvoiceStatus.Unpaid
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Result<Invoice>.Success(invoice);
        }
    }
}
