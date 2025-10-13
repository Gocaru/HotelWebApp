using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HotelWebAppContext _context;
        private readonly IInvoiceRepository _invoiceRepo;

        public PaymentService(HotelWebAppContext context, IInvoiceRepository invoiceRepo)
        {
            _context = context;
            _invoiceRepo = invoiceRepo;
        }

        public async Task<Result<Invoice>> CreateInvoiceForReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.ReservationAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .Include(r => r.ActivityBookings)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
            {
                return Result<Invoice>.Failure("Reservation not found.");
            }

            if (reservation.Status != ReservationStatus.CheckedIn)
            {
                return Result<Invoice>.Failure("Only checked-in reservations can generate invoices.");
            }

            var existingInvoice = await _invoiceRepo.GetByReservationIdAsync(reservationId);

            if (existingInvoice != null)
            {
                return Result<Invoice>.Success(existingInvoice, "Invoice already exists for this reservation.");
            }


            // CALCULAR PREÇO DO QUARTO DO ZERO
            var nights = (reservation.CheckOutDate - reservation.CheckInDate).Days;
            if (nights <= 0) nights = 1;

            decimal roomBasePrice = reservation.Room.PricePerNight * nights;

            // APLICAR DESCONTO SE EXISTIR
            decimal roomPrice = roomBasePrice;
            if (reservation.DiscountPercentage.HasValue && reservation.DiscountPercentage > 0)
            {
                decimal discount = roomBasePrice * (reservation.DiscountPercentage.Value / 100);
                roomPrice = roomBasePrice - discount;
            }

            // AMENITIES
            decimal amenitiesCost = reservation.ReservationAmenities?
                .Sum(ra => ra.PriceAtTimeOfBooking * ra.Quantity) ?? 0;


            if (reservation.ActivityBookings != null)
            {
                foreach (var ab in reservation.ActivityBookings)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {ab.Activity?.Name ?? "Unknown"}: Status={ab.Status}, Price=€{ab.TotalPrice:F2}");
                }
            }

            decimal activitiesCost = reservation.ActivityBookings?
                .Where(ab => ab.Status != ActivityBookingStatus.Cancelled)
                .Sum(ab => ab.TotalPrice) ?? 0;

            System.Diagnostics.Debug.WriteLine($"Activities (non-cancelled): €{activitiesCost:F2}");

            // TOTAL
            decimal totalAmount = roomPrice + amenitiesCost + activitiesCost;

            var invoice = new Invoice
            {
                ReservationId = reservationId,
                GuestId = reservation.GuestId,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = InvoiceStatus.Unpaid
            };

            await _invoiceRepo.CreateAsync(invoice);

            return Result<Invoice>.Success(invoice, "Invoice created successfully.");
        }

        public async Task<Result> ProcessPaymentAsync(int invoiceId, decimal amount, PaymentMethod paymentMethod)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);

            if (invoice == null)
            {
                return Result.Failure("Invoice not found.");
            }

            if (invoice.Status == InvoiceStatus.Paid)
            {
                return Result.Failure("Invoice is already fully paid.");
            }

            var totalPaid = await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .SumAsync(p => p.Amount);

            var remainingBalance = invoice.TotalAmount - totalPaid;

            if (amount > remainingBalance)
            {
                return Result.Failure($"Payment amount exceeds remaining balance of {remainingBalance:C}.");
            }

            var payment = new Payment
            {
                InvoiceId = invoiceId,
                Amount = amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = paymentMethod,
                TransactionId = GenerateTransactionId()
            };

            _context.Payments.Add(payment);

            totalPaid += amount;
            if (totalPaid >= invoice.TotalAmount)
            {
                invoice.Status = InvoiceStatus.Paid;
            }

            await _context.SaveChangesAsync();

            return Result.Success("Payment processed successfully.");
        }

        public async Task<Result<Invoice>> CreateInvoiceForNoShowAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
            {
                return Result<Invoice>.Failure("Reservation not found.");
            }

            if (reservation.Status != ReservationStatus.NoShow)
            {
                return Result<Invoice>.Failure("Only no-show reservations can generate no-show invoices.");
            }

            var existingInvoice = await _invoiceRepo.GetByReservationIdAsync(reservationId);

            if (existingInvoice != null)
            {
                return Result<Invoice>.Success(existingInvoice, "Invoice already exists for this reservation.");
            }

            decimal penaltyAmount = reservation.TotalPrice * 0.5m;

            var invoice = new Invoice
            {
                ReservationId = reservationId,
                GuestId = reservation.GuestId,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = penaltyAmount,
                Status = InvoiceStatus.Unpaid
            };

            await _invoiceRepo.CreateAsync(invoice);

            return Result<Invoice>.Success(invoice, "No-show invoice created successfully.");
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}