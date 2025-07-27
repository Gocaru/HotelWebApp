using HotelWebApp.Data.Entities;

namespace HotelWebApp.Models
{
    public class ReservationDetailsViewModel
    {
        public Reservation Reservation { get; set; }
        public IEnumerable<ChangeRequest> ChangeRequests { get; set; } = new List<ChangeRequest>();
    }
}
