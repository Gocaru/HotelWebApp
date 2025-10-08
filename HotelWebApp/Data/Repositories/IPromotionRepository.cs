using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<IEnumerable<Promotion>> GetAllActiveAsync();
        Task<IEnumerable<Promotion>> GetPromotionsForDateRangeAsync(DateTime checkInDate, DateTime checkOutDate);
        Task<Promotion?> GetByIdAsync(int id);
        Task<Promotion?> GetActiveByIdAsync(int id);
        Task CreateAsync(Promotion promotion);
        Task UpdateAsync(Promotion promotion);
        Task DeleteAsync(int id);
    }
}
