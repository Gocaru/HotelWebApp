using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Controllers
{
    [Authorize(Roles = "Employee, Admin")]
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
            // A única responsabilidade do Controller é chamar o serviço
            var result = await _paymentService.ProcessPaymentAsync(invoiceId, amount, paymentMethod);

            // E depois mostrar o resultado
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction(nameof(Details), new { id = invoiceId });
        }
    }
}
