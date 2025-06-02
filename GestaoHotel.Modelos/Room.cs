namespace HotelManagement.Core
{
    public enum RoomType { Standard, Suite, Luxury }
    public enum RoomStatus { Available, Occupied, UnderMaintenance, Reserved }
    public class Room
    {
        public int RoomId { get; set; } // Identificador único (PK)
        public int Number { get; set; }
        public  RoomType Type { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public RoomStatus Status { get; set; }
    }
}
