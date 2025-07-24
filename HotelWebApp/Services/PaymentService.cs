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

        public async Task<Result> ProcessPaymentAsync(int invoiceId, decimal amount, PaymentMethod paymentMethod)
        {
            // 1. Encontrar a fatura e incluir a Reserva e os Pagamentos existentes
            var invoice = await _context.Invoices
                .Include(i => i.Reservation)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null) return Result.Failure("Invoice not found.");
            if (invoice.Status == InvoiceStatus.Paid) return Result.Failure("Invoice is already fully paid.");
            if (amount <= 0) return Result.Failure("Payment amount must be positive.");

            var totalPaidSoFar = invoice.Payments.Sum(p => p.Amount);
            var balanceDue = invoice.TotalAmount - totalPaidSoFar;

            if (amount > balanceDue)
            {
                return Result.Failure($"Payment of {amount:C} exceeds the balance due of {balanceDue:C}.");
            }

            // 2. Criar o novo registo de pagamento
            var newPayment = new Payment
            {
                InvoiceId = invoiceId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                PaymentDate = DateTime.UtcNow
            };
            _context.Payments.Add(newPayment);

            // 3. Atualizar o status da Fatura com base no novo total pago
            var newTotalPaid = totalPaidSoFar + amount;
            if (newTotalPaid >= invoice.TotalAmount)
            {
                invoice.Status = InvoiceStatus.Paid;
                // 4. Se a fatura foi totalmente paga, complete a reserva
                if (invoice.Reservation != null)
                {
                    invoice.Reservation.Status = ReservationStatus.Completed;
                }
            }
            else
            {
                invoice.Status = InvoiceStatus.PartiallyPaid;
            }

            // 5. Salvar tudo
            await _context.SaveChangesAsync();

            return Result.Success("Payment registered successfully.");
        }
    }
}
