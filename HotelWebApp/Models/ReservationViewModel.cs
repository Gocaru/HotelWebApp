using HotelWebApp.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel used for the Create and Edit reservation forms.
    /// It holds the data being entered or modified by the user, along with
    /// collections to populate dropdown lists.
    /// </summary>
    public class ReservationViewModel
    {
        /// <summary>
        /// The ID of the reservation. Used when editing an existing reservation.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the guest (ApplicationUser) for the reservation.
        /// Binds to the guest selection dropdown, used by employees.
        /// </summary>
        [Display(Name = "Guest")]
        public string? GuestId { get; set; }

        /// <summary>
        /// The ID of the room being booked. Binds to the room selection dropdown.
        /// </summary>
        [Required(ErrorMessage ="Please select a room.")]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        /// <summary>
        /// The check-in date for the reservation.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        /// <summary>
        /// The check-out date for the reservation.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        /// <summary>
        /// The number of guests for this reservation.
        /// </summary>
        [Required]
        [Range(1, 10, ErrorMessage = "Please enter a valid number of guests.")]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; }

        /// <summary>
        /// An optional string to track the navigation source (e.g., "dashboard", "schedule").
        /// Used to provide intelligent "Back" or "Cancel" button navigation.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// The current status of the reservation. Used in the Edit view to display information.
        /// </summary>
        public ReservationStatus? Status { get; set; }

        /// <summary>
        /// A collection of SelectListItem used to populate the guest dropdown for employees.
        /// </summary>
        public IEnumerable<SelectListItem>? Guests { get; set; }

        /// <summary>
        /// A collection of SelectListItem used to populate the available rooms dropdown.
        /// </summary>
        public IEnumerable<SelectListItem>? Rooms { get; set; }
    }
}
