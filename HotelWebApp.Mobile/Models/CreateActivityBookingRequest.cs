using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Models
{
    public class CreateActivityBookingRequest
    {
        public int ActivityId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int NumberOfPeople { get; set; }
        public int ReservationId { get; set; }

        public override string ToString()
        {
            return $"Activity:{ActivityId}, Date:{ScheduledDate:yyyy-MM-dd HH:mm:ss zzz}, People:{NumberOfPeople}, Reservation:{ReservationId}";
        }
    }
}
