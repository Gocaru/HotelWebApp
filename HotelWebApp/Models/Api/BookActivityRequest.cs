using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models.Api
{

    /// <summary>
    /// Request model for booking an activity
    /// </summary>
    public class BookActivityRequest
    {
        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        [Range(1, 50)]
        public int NumberOfPeople { get; set; }

        public int ReservationId { get; set; }
    }
}
