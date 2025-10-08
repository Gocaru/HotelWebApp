using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly HotelWebAppContext _context;

        public ActivityRepository(HotelWebAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Activity>> GetAllAsync()
        {
            return await _context.Activities
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetAllActiveAsync()
        {
            return await _context.Activities
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<Activity?> GetByIdAsync(int id)
        {
            return await _context.Activities
                .Include(a => a.ActivityBookings)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task CreateAsync(Activity activity)
        {
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Activity activity)
        {
            _context.Activities.Update(activity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var activity = await GetByIdAsync(id);
            if (activity != null)
            {
                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasActiveBookingsAsync(int activityId)
        {
            return await _context.ActivityBookings
                .AnyAsync(ab => ab.ActivityId == activityId &&
                               ab.Status != ActivityBookingStatus.Cancelled);
        }
    }
}
