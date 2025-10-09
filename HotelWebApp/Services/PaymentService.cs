using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Services
{
    /// <summary>
    /// Implements the IPaymentService interface to provide business logic for financial operations.
    /// This service interacts directly with the DbContext to ensure transactional integrity.
    /// </summary>
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
                return Result<Invoice>.Success(reservation.Invoice);
            }

            // Calculate amenities total
            decimal amenitiesTotal = reservation.ReservationAmenities.Sum(ra => ra.PriceAtTimeOfBooking * ra.Quantity);

            // Calculate activities total (only Completed activities linked to this reservation)
            decimal activitiesTotal = await _context.ActivityBookings
                .Where(ab => ab.ReservationId == reservationId && ab.Status == ActivityBookingStatus.Completed)
                .SumAsync(ab => ab.TotalPrice);

            // Calculate final amount including activities
            decimal finalInvoiceAmount = reservation.TotalPrice + amenitiesTotal + activitiesTotal;

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

        public async Task<Result> ProcessPaymentAsync(int invoiceId, decimal amount, PaymentMethod paymentMethod)
        {
            // Eagerly load the Reservation and existing Payments to perform all business logic.
            var invoice = await _context.Invoices
                .Include(i => i.Reservation)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            // --- Business Rule Validations ---
            if (invoice == null) return Result.Failure("Invoice not found.");
            if (invoice.Status == InvoiceStatus.Paid) return Result.Failure("Invoice is already fully paid.");
            if (amount <= 0) return Result.Failure("Payment amount must be positive.");

            var totalPaidSoFar = invoice.Payments.Sum(p => p.Amount);
            var balanceDue = invoice.TotalAmount - totalPaidSoFar;

            if (amount > balanceDue)
            {
                return Result.Failure($"Payment of {amount:C} exceeds the balance due of {balanceDue:C}.");
            }
            // --- End of Validations ---

            // Create the new payment record.
            var newPayment = new Payment
            {
                InvoiceId = invoiceId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                PaymentDate = DateTime.UtcNow
            };
            _context.Payments.Add(newPayment);

            // Update the invoice status based on the new total amount paid.
            var newTotalPaid = totalPaidSoFar + amount;
            if (newTotalPaid >= invoice.TotalAmount)
            {
                invoice.Status = InvoiceStatus.Paid;

                // If the invoice is fully paid, also mark the reservation lifecycle as complete.
                if (invoice.Reservation != null)
                {
                    invoice.Reservation.Status = ReservationStatus.Completed;
                }
            }
            else
            {
                invoice.Status = InvoiceStatus.PartiallyPaid;
            }

            // Save all changes (new Payment, updated Invoice, updated Reservation) in a single transaction.
            await _context.SaveChangesAsync();

            return Result.Success("Payment registered successfully.");
        }

        public async Task<Result<Invoice>> CreateInvoiceForNoShowAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Invoice)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null) return Result<Invoice>.Failure("Reservation not found.");
            if (reservation.Invoice != null) return Result<Invoice>.Success(reservation.Invoice); // Já existe

            decimal noShowFee = reservation.Room?.PricePerNight ?? 0;

            if (noShowFee <= 0)
            {
                return Result<Invoice>.Success(null, "No-show fee is zero, invoice not created.");
            }

            var invoice = new Invoice
            {
                ReservationId = reservation.Id,
                GuestId = reservation.GuestId,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = noShowFee,
                Status = InvoiceStatus.Unpaid
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Result<Invoice>.Success(invoice);
        }
    }
}
