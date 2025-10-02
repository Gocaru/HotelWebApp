namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Represents an amenity or service included in a reservation
    /// </summary>
    public class ReservationAmenityDto
    {
        public string AmenityName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
