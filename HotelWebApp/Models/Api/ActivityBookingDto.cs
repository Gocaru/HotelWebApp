namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Represents a booking for a hotel activity
    /// </summary>
    public class ActivityBookingDto
    {
        public int Id { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int NumberOfPeople { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
    }
}
