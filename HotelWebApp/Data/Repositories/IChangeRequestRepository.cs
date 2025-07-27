using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    public interface IChangeRequestRepository
    {
        Task CreateAsync(ChangeRequest changeRequest);
        
        Task<IEnumerable<ChangeRequest>> GetPendingRequestsAsync();

        Task<ChangeRequest?> GetPendingRequestForReservationAsync(int reservationId);

        Task<ChangeRequest?> GetByIdAsync(int id);
        
        Task UpdateAsync(ChangeRequest changeRequest);

        Task<IEnumerable<ChangeRequest>> GetRequestsForReservationAsync(int reservationId);
    }
}
