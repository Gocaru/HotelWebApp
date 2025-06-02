using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementWpfApp_.Models
{
    public class DashboardStats
    {
        public int TotalGuests { get; set; }
        public int TotalRooms { get; set; }
        public int TotalBookings { get; set; }
        public int TotalInvoices { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
