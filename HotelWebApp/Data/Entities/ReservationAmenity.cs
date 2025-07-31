using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents the join entity for the many-to-many relationship
    /// between Reservation and Amenity. Each instance signifies that a specific
    /// amenity has been added to a specific reservation.
    /// </summary>
    public class ReservationAmenity : IEntity
    {
        /// <summary>
        /// The unique identifier for this specific booking of an amenity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the Reservation.
        /// </summary>
        [Required]
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }


        /// <summary>
        /// Foreign key to the Amenity.
        /// </summary>
        [Required]
        public int AmenityId { get; set; } 
        public Amenity Amenity { get; set; }

        /// <summary>
        /// The quantity of the amenity added to the reservation (e.g., 2 breakfasts).
        /// </summary>
        [Required]
        [Range(1, 100, ErrorMessage = "The quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// The price of the amenity at the moment it was added to the reservation.
        /// This is crucial for historical accuracy, as amenity prices can change over time.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtTimeOfBooking { get; set; }

        /// <summary>
        /// The date and time when this amenity was added to the reservation.
        /// </summary>
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
