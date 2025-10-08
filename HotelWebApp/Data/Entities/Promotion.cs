using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents a promotional offer or special deal
    /// </summary>
    public class Promotion : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? DiscountPercentage { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ImageUrl { get; set; }

        public string? Terms { get; set; }

        public bool IsExpired => EndDate < DateTime.Today;

        public PromotionType Type { get; set; } = PromotionType.General;
        public int? MinimumNights { get; set; }        // Para LongStay: estadia mínima
        public int? MinimumDaysInAdvance { get; set; }  // Para EarlyBird: antecedência mínima
    }
}
