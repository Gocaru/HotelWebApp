using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    public interface IActivityRepository
    {
        Task<IEnumerable<Activity>> GetAllAsync();
        Task<IEnumerable<Activity>> GetAllActiveAsync();
        Task<Activity?> GetByIdAsync(int id);
        Task CreateAsync(Activity activity);
        Task UpdateAsync(Activity activity);
        Task DeleteAsync(int id);
        Task<bool> HasActiveBookingsAsync(int activityId);
    }
}
