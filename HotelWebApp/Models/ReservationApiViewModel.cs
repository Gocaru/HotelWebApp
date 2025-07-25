namespace HotelWebApp.Models
{
    public class ReservationApiViewModel
    {
        public int ReservationId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }
    }
}
