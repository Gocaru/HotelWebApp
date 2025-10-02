using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents an activity or service available at the hotel (e.g., Spa, Gym, Pool, Tours)
    /// </summary>
    public class Activity : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Activity Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string? Duration { get; set; }

        [MaxLength(100)]
        public string? Schedule { get; set; }

        public int Capacity { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ImageUrl { get; set; }

        public virtual ICollection<ActivityBooking> ActivityBookings { get; set; } = new HashSet<ActivityBooking>();
    }
}
