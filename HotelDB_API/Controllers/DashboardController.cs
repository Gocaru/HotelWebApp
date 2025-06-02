using HotelDB_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelDB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly HotelContext _context;

        public DashboardController(HotelContext context)
        {
            _context = context;
        }

        // GET: api/Dashboard
        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalGuests = await _context.Guests.CountAsync();
            var totalRooms = await _context.Rooms.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var totalInvoices = await _context.Invoices.CountAsync();

            var totalRevenue = await _context.Invoices
                .SumAsync(i => i.StayTotal + i.ExtrasTotal);

            var stats = new
            {
                TotalGuests = totalGuests,
                TotalRooms = totalRooms,
                TotalBookings = totalBookings,
                TotalInvoices = totalInvoices,
                TotalRevenue = totalRevenue
            };

            return Ok(stats);
        }
    }
}
