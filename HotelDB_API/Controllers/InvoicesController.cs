using HotelDB_API.Data;
using HotelManagement.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelDB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly HotelContext _context;

        public InvoicesController(HotelContext context)
        {
            _context = context;
        }

        // GET: api/Invoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            var invoices = await _context.Invoices.ToListAsync();

            // Adicionar GuestName dinamicamente (simples)
            foreach (var invoice in invoices)
            {
                var booking = await _context.Bookings.FindAsync(invoice.BookingId);
                var guest = booking != null
                    ? await _context.Guests.FindAsync(booking.GuestId)
                    : null;

                invoice.GuestName = guest?.Name ?? "Unknown";
            }

            return invoices;
        }

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
                return NotFound();

            var booking = await _context.Bookings.FindAsync(invoice.BookingId);
            var guest = booking != null
                ? await _context.Guests.FindAsync(booking.GuestId)
                : null;

            invoice.GuestName = guest?.Name ?? "Unknown";

            return invoice;
        }

        // POST: api/Invoices
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            var extras = await _context.ExtraServices
                .Where(e => e.BookingId == invoice.BookingId)
                .ToListAsync();

            invoice.ExtrasTotal = extras.Sum(e => e.Price);
            invoice.IssueDate = DateTime.Now;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceId }, invoice);
        }

        // PUT: api/Invoices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if (id != invoice.InvoiceId)
                return BadRequest("ID mismatch.");

            var extras = await _context.ExtraServices
                .Where(e => e.BookingId == invoice.BookingId)
                .ToListAsync();

            invoice.ExtrasTotal = extras.Sum(e => e.Price);
            invoice.IssueDate = DateTime.Now;

            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Invoices.Any(i => i.InvoiceId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Invoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
                return NotFound();

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
