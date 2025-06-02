using HotelDB_API.Data;
using HotelManagement.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelDB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestsController : ControllerBase
    {
        private readonly HotelContext _context;

        public GuestsController(HotelContext context)
        {
            _context = context;
        }

        // GET: api/guests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guest>>> GetGuests()
        {
            return await _context.Guests.ToListAsync();
        }

        // GET: api/guests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetGuest(int id)
        {
            var guest = await _context.Guests.FindAsync(id);

            if (guest == null)
            {
                return NotFound();
            }

            return guest;
        }

        // POST: api/guests
        [HttpPost]
        public async Task<ActionResult<Guest>> PostGuest(Guest guest)
        {
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGuest), new { id = guest.GuestId }, guest);
        }

        // PUT: api/guests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGuest(int id, Guest guest)
        {
            if (id != guest.GuestId)
            {
                return BadRequest("ID mismatch.");
            }

            _context.Entry(guest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Guests.Any(e => e.GuestId == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE: api/guests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                return NotFound();
            }

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
