using HotelWebApp.Data.Entities;
using HotelWebApp.Models;

namespace HotelWebApp.Services
{
    /// <summary>
    /// Defines the contract for the payment processing service.
    /// This service handles business logic related to creating invoices and processing payments.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Creates a new invoice for a specific reservation or retrieves the existing one.
        /// This is typically called during the check-out process.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to generate an invoice for.</param>
        /// <returns>A Result object containing the created or existing Invoice on success, or an error message on failure.</returns>
        Task<Result<Invoice>> CreateInvoiceForReservationAsync(int reservationId);

        /// <summary>
        /// Processes a payment against a specific invoice.
        /// This operation creates a Payment record, updates the invoice status (e.g., to Paid or PartiallyPaid),
        /// and, if fully paid, updates the associated reservation's status to 'Completed'.
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice being paid.</param>
        /// <param name="amount">The amount being paid in this transaction.</param>
        /// <param name="paymentMethod">The method of payment used.</param>
        /// <returns>A Result object indicating the outcome of the payment process.</returns>
        Task<Result> ProcessPaymentAsync(int invoiceId, decimal amount, PaymentMethod paymentMethod);

        Task<Result<Invoice>> CreateInvoiceForNoShowAsync(int reservationId);
    }
}
