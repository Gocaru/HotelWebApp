using HotelDB_WPF_Framework.Enums;

namespace HotelDB_WPF_Framework.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public int Number { get; set; }
        public RoomType Type { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public RoomStatus Status { get; set; }
    }
}
