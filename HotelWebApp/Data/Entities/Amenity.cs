using HotelWebApp.Data.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents an extra service or product that can be added to a reservation,
    /// such as breakfast, spa access, or a welcome package.
    /// </summary>
    public class Amenity : IEntity
    {
        /// <summary>
        /// The unique identifier for the amenity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The public-facing name of the amenity (e.g., "Breakfast Buffet", "Pet-Friendly Kit").
        /// </summary>
        [Required(ErrorMessage = "The amenity name is required.")]
        [StringLength(100)]
        [Display(Name = "Amenity Name")]
        public string Name { get; set; }

        /// <summary>
        /// An optional, more detailed description of what the amenity includes.
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// The price of the amenity per unit.
        /// </summary>
        [Required(ErrorMessage = "The price is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price")]
        [Range(0.01, 10000.00, ErrorMessage = "The price must be greater than zero.")]
        public decimal Price { get; set; }

        /// <summary>
        /// Navigation property for the join entity connecting Amenities to Reservations.
        /// Represents all the times this specific amenity has been booked.
        /// </summary>
        public ICollection<ReservationAmenity> ReservationAmenities { get; set; } = new HashSet<ReservationAmenity>();
        
    }
}
