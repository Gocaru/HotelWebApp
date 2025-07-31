namespace HotelWebApp.Models
{
    /// <summary>
    /// Data Transfer Object (DTO) used for representing reservation data in the Web API.
    /// This model exposes a curated set of properties, ensuring that sensitive or unnecessary
    /// data from the domain entities is not sent to the client.
    /// </summary>
    public class ReservationApiViewModel
    {
        /// <summary>
        /// The unique ID of the reservation.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// The number of the room associated with the reservation.
        /// </summary>
        public string RoomNumber { get; set; }

        /// <summary>
        /// The type of the room (e.g., Standard, Suite).
        /// </summary>
        public string RoomType { get; set; }

        /// <summary>
        /// The check-in date for the reservation.
        /// </summary>
        public DateTime CheckInDate { get; set; }

        /// <summary>
        /// The check-out date for the reservation.
        /// </summary>
        public DateTime CheckOutDate { get; set; }

        /// <summary>
        /// The calculated total cost of the reservation, including the room and any added services.
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// The current status of the reservation (e.g., "Confirmed", "CheckedIn").
        /// </summary>
        public string Status { get; set; }
    }
}
