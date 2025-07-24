using HotelWebApp.Data.Entities;

namespace HotelWebApp.Models
{
    public class ReservationDetailsViewModel
    {
        public Reservation Reservation { get; set; }
        public ChangeRequest? PendingChangeRequest { get; set; }
    }
}
