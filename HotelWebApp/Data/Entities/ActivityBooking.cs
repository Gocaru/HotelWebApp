using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents a guest's booking for a specific activity
    /// </summary>
    public class ActivityBooking : IEntity
    {
        public int Id { get; set; }

        [Required]
        public int ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }

        [Required]
        public string GuestId { get; set; }

        [ForeignKey("GuestId")]
        public virtual ApplicationUser Guest { get; set; }

        public int? ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public virtual Reservation? Reservation { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        public int NumberOfPeople { get; set; } = 1;

        [Required]
        public ActivityBookingStatus Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
    }

    public enum ActivityBookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3
    }
}
