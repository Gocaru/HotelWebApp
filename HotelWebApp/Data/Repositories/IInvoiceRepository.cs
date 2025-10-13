using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(int id);
        Task<Invoice?> GetByReservationIdAsync(int reservationId);
        Task CreateAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
    }
}
