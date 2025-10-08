namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Represents a room reservation made by a guest
    /// </summary>
    public class ReservationDto
    {
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime ReservationDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public int NumberOfGuests { get; set; }
        public RoomDto? Room { get; set; }
        public List<ReservationAmenityDto> Amenities { get; set; } = new();

        public decimal? OriginalPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string? PromotionTitle { get; set; }
    }
}
