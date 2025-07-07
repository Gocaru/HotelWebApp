using HotelWebApp.Data.Entities;
using HotelWebApp.Models;

namespace HotelWebApp.Services
{
    public interface IPaymentService
    {
        Task<Result<Invoice>> CreateInvoiceForReservationAsync(int reservationId);
    }
}
