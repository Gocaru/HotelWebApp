using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Core
{
    public enum ExtraServiceType { Breakfast, SpaAccess, PrivateParking, RoomService}
    public class ExtraService
    {
        public int ExtraServiceId { get; set; }          
        public int BookingId { get; set; }         
        public ExtraServiceType ServiceType { get; set; }   
        public decimal Price { get; set; }
    }

}
