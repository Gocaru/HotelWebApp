namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Represents detailed information about a hotel activity available for booking
    /// </summary>
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Duration { get; set; }
        public string? Schedule { get; set; }
        public int Capacity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
