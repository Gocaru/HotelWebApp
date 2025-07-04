using HotelWebApp.Data.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    public class Amenity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The amenity name is required.")]
        [StringLength(100)]
        [Display(Name = "Amenity Name")]
        public string Name { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The price is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price")]
        [Range(0.01, 10000.00, ErrorMessage = "The price must be greater than zero.")]
        public decimal Price { get; set; }

        public ICollection<ReservationAmenity> ReservationAmenities { get; set; } = new HashSet<ReservationAmenity>();

    }
}
