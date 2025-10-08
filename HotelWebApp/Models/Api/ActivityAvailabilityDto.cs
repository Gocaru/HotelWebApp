namespace HotelWebApp.Models.Api
{
    public class ActivityAvailabilityDto
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int Capacity { get; set; }
        public int CurrentParticipants { get; set; }
        public int AvailableSpots { get; set; }
        public bool IsFull { get; set; }
    }
}
