using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Messages
{
    public class ActivityBookedMessage
    {
        public int ActivityBookingId { get; set; }
        public string ActivityName { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
