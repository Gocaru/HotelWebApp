using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Request model for creating a new room reservation
    /// </summary>
    public class CreateReservationRequest
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, 10)]
        public int NumberOfGuests { get; set; }
    }
}
