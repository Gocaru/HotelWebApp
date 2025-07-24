using HotelWebApp.Data.Entities;

namespace HotelWebApp.Models
{
    public class HomeDashboardViewModel
    {
        public IEnumerable<Reservation> CheckInsToday { get; set; }
        public IEnumerable<Reservation> CheckOutsToday { get; set; }

        public IEnumerable<ChangeRequest> PendingChangeRequests { get; set; }
    }
}
