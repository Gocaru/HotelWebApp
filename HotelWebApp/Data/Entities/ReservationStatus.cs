using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Data.Entities
{
    public enum ReservationStatus
    {
        [Display(Name = "Confirmed")]
        Confirmed = 0,

        [Display(Name = "Checked-In")]
        CheckedIn = 1,

        [Display(Name = "Checked-Out")]
        CheckedOut = 2,

        [Display(Name = "Cancelled")]
        Cancelled = 3,

        [Display(Name = "No-Show")]
        NoShow = 4

    }
}
