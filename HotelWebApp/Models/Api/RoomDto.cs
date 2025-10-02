namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Represents detailed information about a hotel room
    /// </summary>
    public class RoomDto
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string? ImageUrl { get; set; }
        public string? Status { get; set; }
    }
}
