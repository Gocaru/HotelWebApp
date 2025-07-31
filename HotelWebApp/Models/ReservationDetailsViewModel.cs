using HotelWebApp.Data.Entities;

namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel for the Reservation Details page.
    /// It aggregates the core Reservation entity along with related data,
    /// such as the history of change requests, to provide a complete view.
    /// </summary>
    public class ReservationDetailsViewModel
    {
        /// <summary>
        /// The main Reservation entity, including all its related data
        /// (Guest, Room, Amenities, Invoice).
        /// </summary>
        public Reservation Reservation { get; set; }

        /// <summary>
        /// A collection of all change requests (pending, approved, or rejected)
        /// associated with this reservation. Used to display the request history.
        /// </summary>
        public IEnumerable<ChangeRequest> ChangeRequests { get; set; } = new List<ChangeRequest>();
    }
}
