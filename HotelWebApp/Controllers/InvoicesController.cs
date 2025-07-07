using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Controllers
{
    [Authorize(Roles = "Employee, Admin")]
    public class InvoicesController : Controller
    {
        private readonly HotelWebAppContext _context;

        public InvoicesController(HotelWebAppContext context)
        {
            _context = context;
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Vamos buscar a fatura e todos os dados relacionados para mostrar na página
            var invoice = await _context.Invoices
                .Include(i => i.Reservation)
                .ThenInclude(r => r.Room)
                .Include(i => i.Guest)
                .Include(i => i.Payments) // Inclui os pagamentos já feitos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/AddPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPayment(int invoiceId, decimal amount, PaymentMethod paymentMethod)
        {
            var invoice = await _context.Invoices.Include(i => i.Payments).FirstOrDefaultAsync(i => i.Id == invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Payment amount must be positive.";
                return RedirectToAction(nameof(Details), new { id = invoiceId });
            }

            // Cria um novo registo de pagamento
            var payment = new Payment
            {
                InvoiceId = invoiceId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            // Atualiza o status da fatura com base no total pago
            var totalPaid = invoice.Payments.Sum(p => p.Amount) + amount;
            if (totalPaid >= invoice.TotalAmount)
            {
                invoice.Status = InvoiceStatus.Paid;
            }
            else
            {
                invoice.Status = InvoiceStatus.PartiallyPaid;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Payment registered successfully!";
            return RedirectToAction(nameof(Details), new { id = invoiceId });
        }
    }
}
