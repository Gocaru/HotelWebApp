using HotelDB_WPF_Framework.Enums;

namespace HotelDB_WPF_Framework.Models
{
    public class ExtraService
    {
        public int ExtraServiceId { get; set; }
        public int BookingId { get; set; }
        public ExtraServiceType ServiceType { get; set; }
        public decimal Price { get; set; }
    }
}
