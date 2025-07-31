using Elfie.Serialization;
using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Controllers
{
    /// <summary>
    /// Manages financial invoices related to reservations.
    /// Allows employees to view invoice details and process payments.
    /// Accessible only by users with 'Employee' role.
    /// </summary>
    [Authorize(Roles = "Employee")]
    public class InvoicesController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly HotelWebAppContext _context;

        public InvoicesController(IPaymentService paymentService, HotelWebAppContext context)
        {
            _paymentService = paymentService;
            _context = context;
        }

        // GET: Invoices/Details/5
        /// <summary>
        /// Displays a detailed view of a single invoice, including the associated reservation,
        /// guest, and payment history.
        /// </summary>
        /// <param name="id">The ID of the invoice to display.</param>
        /// /// <param name="reservationId">The ID of the associated reservation.</param>
        /// /// <param name="source">Optional. The navigation origin (e.g., "dashboard").</param>
        /// <returns>The details view for the invoice, or NotFound if it doesn't exist.</returns>
        public async Task<IActionResult> Details(int? id, [FromQuery]  int? reservationId, string source)
        {
            if (id == null && reservationId == null)
            {
                return NotFound();
            }

            var query = _context.Invoices
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.Room)
                .Include(i => i.Guest)
                .Include(i => i.Payments)
                .AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(i => i.Id == id.Value);
            }
            else
            {
                query = query.Where(i => i.ReservationId == reservationId.Value);
            }

            var invoice = await query.FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound();
            }

            ViewBag.Source = source;
            return View(invoice);
        }

        // POST: Invoices/AddPayment
        /// <summary>
        /// Processes a payment for a given invoice.
        /// This action delegates the business logic to the PaymentService.
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice being paid.</param>
        /// <param name="amount">The amount being paid.</param>
        /// <param name="paymentMethod">The method of payment (e.g., Cash, CreditCard).</param>
        /// /// /// <param name="source">Optional. The navigation origin (e.g., "dashboard").</param>
        /// <returns>Redirects back to the invoice details page with a success or error message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPayment(int invoiceId, decimal amount, PaymentMethod paymentMethod, string source)
        {
            // Delegate the complex business logic (creating payment, updating statuses) to the service layer.
            var result = await _paymentService.ProcessPaymentAsync(invoiceId, amount, paymentMethod);

            // Use the result from the service to provide feedback to the user via TempData.
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
            }

            // Redirect back to the details page to show the updated invoice status and payment history.
            return RedirectToAction(nameof(Details), new { id = invoiceId, source = source });
        }
    }
}
