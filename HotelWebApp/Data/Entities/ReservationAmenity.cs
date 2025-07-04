using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    public class ReservationAmenity
    {
        public int Id { get; set; }

        [Required]
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        [Required]
        public int AmenityId { get; set; } // Chave estrangeira para Amenity
        public Amenity Amenity { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "The quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtTimeOfBooking { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
