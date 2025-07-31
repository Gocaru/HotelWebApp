namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel specifically designed for displaying reservations in a list or grid format (e.g., the employee's Reservation Management page).
    /// It contains pre-formatted and calculated properties to simplify the view's rendering logic.
    /// </summary>
    public class ReservationListViewModel
    {
        /// <summary>
        /// The unique ID of the reservation.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The full name of the guest.
        /// </summary>
        public string GuestName { get; set; }

        /// <summary>
        /// A formatted string combining the guest's name and the number of guests (e.g., "John Doe (2 pax)").
        /// </summary>
        public string GuestDetails { get; set; }

        /// <summary>
        /// The number of the reserved room.
        /// </summary>
        public string RoomNumber { get; set; }

        /// <summary>
        /// A formatted string combining the room number and type (e.g., "101 (Suite)").
        /// </summary>
        public string RoomDetails { get; set; }

        /// <summary>
        /// The check-in date of the reservation.
        /// </summary>
        public DateTime CheckInDate { get; set; }

        /// <summary>
        /// The check-out date of the reservation.
        /// </summary>
        public DateTime CheckOutDate { get; set; }

        /// <summary>
        /// The number of guests for the reservation.
        /// </summary>
        public int NumberOfGuests { get; set; }

        /// <summary>
        /// The final calculated total cost for the stay, including room and amenities.
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// The display-friendly text for the reservation's status (e.g., "Checked-In").
        /// </summary>
        public string StatusText { get; set; }

        /// <summary>
        /// The Bootstrap CSS class for the status badge's color (e.g., "bg-success").
        /// This is pre-calculated in the controller to simplify the view.
        /// </summary>
        public string StatusBadgeClass { get; set; }

        public bool IsOverdue { get; set; }

        // --- UI Logic Properties ---

        /// <summary>
        /// A boolean flag indicating if the 'Check-in' action is currently available for this reservation.
        /// </summary>
        public bool CanCheckIn { get; set; }

        /// <summary>
        /// A boolean flag indicating if the 'Check-out' action is currently available for this reservation.
        /// </summary>
        public bool CanCheckOut { get; set; }

        /// <summary>
        /// A boolean flag indicating if the reservation can be edited.
        /// </summary>
        public bool CanEdit { get; set; }

        /// <summary>
        /// A boolean flag indicating if the reservation can be deleted.
        /// </summary>
        public bool CanDelete { get; set; }
    }
}
