using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly HotelWebAppContext _context;

        public PromotionRepository(HotelWebAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync()
        {
            return await _context.Promotions
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Promotion>> GetAllActiveAsync()
        {
            return await _context.Promotions
                .Where(p => p.IsActive && p.EndDate >= DateTime.Today)
                .OrderBy(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<Promotion?> GetByIdAsync(int id)
        {
            return await _context.Promotions.FindAsync(id);
        }

        public async Task CreateAsync(Promotion promotion)
        {
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var promotion = await GetByIdAsync(id);
            if (promotion != null)
            {
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Promotion?> GetActiveByIdAsync(int id)
        {
            return await _context.Promotions
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<IEnumerable<Promotion>> GetPromotionsForDateRangeAsync(DateTime checkInDate, DateTime checkOutDate)
        {
            var numberOfNights = (checkOutDate - checkInDate).Days;
            var today = DateTime.Today;
            var daysInAdvance = (checkInDate - today).Days;

            var isWeekendStay = checkInDate.DayOfWeek == DayOfWeek.Friday ||
                                checkInDate.DayOfWeek == DayOfWeek.Saturday;

            System.Diagnostics.Debug.WriteLine("=== FILTERING PROMOTIONS ===");
            System.Diagnostics.Debug.WriteLine($"Check-in: {checkInDate:yyyy-MM-dd} ({checkInDate.DayOfWeek})");
            System.Diagnostics.Debug.WriteLine($"Check-out: {checkOutDate:yyyy-MM-dd}");
            System.Diagnostics.Debug.WriteLine($"Number of nights: {numberOfNights}");
            System.Diagnostics.Debug.WriteLine($"Days in advance: {daysInAdvance}");
            System.Diagnostics.Debug.WriteLine($"Is Weekend Stay: {isWeekendStay}");

            var allPromotions = await _context.Promotions
                .Where(p => p.IsActive
                    && p.StartDate <= checkInDate
                    && p.EndDate >= checkInDate)
                .ToListAsync();

            System.Diagnostics.Debug.WriteLine($"\nFound {allPromotions.Count} active promotions in date range:");

            var filtered = allPromotions.Where(p =>
            {
                // General - sempre aplicável
                if (p.Type == PromotionType.General)
                {
                    System.Diagnostics.Debug.WriteLine($"  ✅ {p.Title} - General");
                    return true;
                }

                // Weekend - sexta ou sábado
                if (p.Type == PromotionType.Weekend)
                {
                    bool applies = isWeekendStay;
                    System.Diagnostics.Debug.WriteLine($"  {(applies ? "✅" : "❌")} {p.Title} - Weekend (Fri/Sat: {isWeekendStay})");
                    return applies;
                }

                // Weekday - segunda a quinta
                if (p.Type == PromotionType.Weekday)
                {
                    bool applies = !isWeekendStay && checkInDate.DayOfWeek != DayOfWeek.Sunday;
                    System.Diagnostics.Debug.WriteLine($"  {(applies ? "✅" : "❌")} {p.Title} - Weekday");
                    return applies;
                }

                // LongStay - usar MinimumNights
                if (p.Type == PromotionType.LongStay)
                {
                    var minNights = p.MinimumNights ?? 1;
                    bool applies = numberOfNights >= minNights;
                    System.Diagnostics.Debug.WriteLine($"  {(applies ? "✅" : "❌")} {p.Title} - LongStay ({numberOfNights} nights vs {minNights} required)");
                    return applies;
                }

                // EarlyBird - usar MinimumDaysInAdvance
                if (p.Type == PromotionType.EarlyBird)
                {
                    var minDaysAdvance = p.MinimumDaysInAdvance ?? 30;
                    bool applies = daysInAdvance >= minDaysAdvance;
                    System.Diagnostics.Debug.WriteLine($"  {(applies ? "✅" : "❌")} {p.Title} - EarlyBird ({daysInAdvance} days vs {minDaysAdvance} required)");
                    return applies;
                }

                return false;
            }).ToList();

            System.Diagnostics.Debug.WriteLine($"\n✅ Final: {filtered.Count} valid promotions");

            return filtered.OrderByDescending(p => p.DiscountPercentage);
        }
    }
}
