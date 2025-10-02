namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Represents a promotional offer or special deal available at the hotel
    /// </summary>
    public class PromotionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? DiscountPercentage { get; set; }
        public string? ImageUrl { get; set; }
        public string? Terms { get; set; }
    }
}
