using HotelDB_API.Data;
using HotelManagement.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelDB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtraServicesController : ControllerBase
    {
        private readonly HotelContext _context;

        public ExtraServicesController(HotelContext context)
        {
            _context = context;
        }

        // GET: api/ExtraServices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExtraService>>> GetExtraServices()
        {
            return await _context.ExtraServices.ToListAsync();
        }

        // GET: api/ExtraServices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExtraService>> GetExtraService(int id)
        {
            var extra = await _context.ExtraServices.FindAsync(id);

            if (extra == null)
                return NotFound();

            return extra;
        }

        // GET: api/ExtraServices/ByBooking/1
        [HttpGet("ByBooking/{bookingId}")]
        public async Task<ActionResult<IEnumerable<ExtraService>>> GetExtrasByBooking(int bookingId)
        {
            var extras = await _context.ExtraServices
                .Where(e => e.BookingId == bookingId)
                .ToListAsync();

            return extras;
        }

        // POST: api/ExtraServices
        [HttpPost]
        public async Task<ActionResult<ExtraService>> PostExtraService(ExtraService extra)
        {
            _context.ExtraServices.Add(extra);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExtraService), new { id = extra.ExtraServiceId }, extra);
        }

        // PUT: api/ExtraServices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExtraService(int id, ExtraService extra)
        {
            if (id != extra.ExtraServiceId)
                return BadRequest("ID mismatch.");

            _context.Entry(extra).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ExtraServices.Any(e => e.ExtraServiceId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/ExtraServices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExtraService(int id)
        {
            var extra = await _context.ExtraServices.FindAsync(id);

            if (extra == null)
                return NotFound();

            _context.ExtraServices.Remove(extra);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
