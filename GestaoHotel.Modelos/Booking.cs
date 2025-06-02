using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Core
{
    public enum BookingStatus { Reserved, CheckedIn, CheckedOut, Cancelled, Completed }
    public class Booking
    {
        public int BookingId { get; set; }
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public BookingStatus Status { get; set; }

    }
}
