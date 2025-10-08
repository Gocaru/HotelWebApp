using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents a booking of a specific room for a specific guest over a defined period.
    /// This is the core entity of the hotel management system.
    /// </summary>
    public class Reservation : IEntity
    {
        /// <summary>
        /// The unique identifier for the reservation.
        /// </summary>
        public int Id { get ; set ; }

        /// <summary>
        /// The scheduled date and time for the guest's arrival and check-in.
        /// </summary>
        [Required(ErrorMessage ="Check-in date is required.")]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set ; }

        /// <summary>
        /// The scheduled date and time for the guest's departure and check-out.
        /// </summary>
        [Required(ErrorMessage = "Check-out date is required.")]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; }

        /// <summary>
        /// The date and time when the reservation was created in the system.
        /// </summary>
        [Display(Name = "Reservation Date")]
        public DateTime ReservationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The calculated total price for the entire stay, including room charges and amenities.
        /// This value may be recalculated if amenities are added/removed.
        /// </summary>
        [Required(ErrorMessage = "Total price is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Price")]
        public decimal TotalPrice {  get; set ; }

        /// <summary>
        /// The current status of the reservation in its lifecycle (e.g., Confirmed, CheckedIn, Completed).
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public ReservationStatus Status { get; set ; }


        /// <summary>
        /// The number of guests staying under this reservation.
        /// </summary>
        [Required]
        [Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10.")]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; }

        /// <summary>
        /// The foreign key for the Room associated with this reservation.
        /// </summary>
        [Required]
        public int RoomId { get; set ; }

        /// <summary>
        /// Navigation property to the booked Room.
        /// </summary>
        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set ; }

        /// <summary>
        /// The foreign key for the Guest (ApplicationUser) who made the reservation.
        /// </summary>
        [Required]
        public string GuestId { get; set ; }

        /// <summary>
        /// Navigation property to the associated Guest.
        /// </summary>
        [ForeignKey("GuestId")]
        public virtual ApplicationUser? ApplicationUser { get; set ; }

        /// <summary>
        /// Navigation property to the collection of extra services (amenities) added to this reservation.
        /// </summary>
        public ICollection<ReservationAmenity> ReservationAmenities { get; set; } = new HashSet<ReservationAmenity>();


        /// <summary>
        /// Navigation property to the financial invoice associated with this reservation.
        /// This may be null until the check-out process is initiated.
        /// </summary>
        public virtual Invoice? Invoice { get; set; }

        /// <summary>
        /// The ID of the promotion applied to this reservation, if any.
        /// </summary>
        public int? PromotionId { get; set; }

        /// <summary>
        /// Navigation property to the applied Promotion.
        /// </summary>
        [ForeignKey("PromotionId")]
        public virtual Promotion? Promotion { get; set; }

        /// <summary>
        /// The discount percentage that was applied from the promotion (stored for historical record).
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal? DiscountPercentage { get; set; }

        /// <summary>
        /// The original price before any discount was applied.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OriginalPrice { get; set; }
    }
}
