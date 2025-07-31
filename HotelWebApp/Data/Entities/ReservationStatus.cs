using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Defines the various stages in the lifecycle of a reservation.
    /// </summary>
    public enum ReservationStatus
    {
        /// <summary>
        /// The reservation has been successfully created and the room is blocked for the guest.
        /// This is the initial state for a new booking.
        /// </summary>
        [Display(Name = "Confirmed")]
        Confirmed = 0,

        /// <summary>
        /// The guest has arrived at the hotel and has officially checked in.
        /// The room is now occupied.
        /// </summary>
        [Display(Name = "Checked-In")]
        CheckedIn = 1,

        /// <summary>
        /// The guest has completed their stay and vacated the room.
        /// Payment is now pending.
        /// </summary>
        [Display(Name = "Checked-Out")]
        CheckedOut = 2,

        /// <summary>
        /// The reservation was cancelled by the guest or staff before the check-in date.
        /// The room is available again.
        /// </summary>
        [Display(Name = "Cancelled")]
        Cancelled = 3,

        /// <summary>
        /// The guest did not arrive on the scheduled check-in date.
        /// The room has been released. A penalty fee may apply.
        /// </summary>
        [Display(Name = "No-Show")]
        NoShow = 4,

        /// <summary>
        /// The guest has checked out and the corresponding invoice has been fully paid.
        /// This is the final state of a successful reservation.
        /// </summary>
        [Display(Name = "Completed")]
        Completed = 5

    }
}
