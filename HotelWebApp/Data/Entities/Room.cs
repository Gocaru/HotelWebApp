using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    public class Room : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Room Number")]
        [MaxLength(10, ErrorMessage ="{0} cannot exceed {1} characters.")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Room Type")]
        public RoomType Type { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Range(1, 10, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int Capacity { get; set; }


        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Price per Night")]
        [Range(0, 1000, ErrorMessage = "{0} must be between {1} and {2}.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Room Status")]
        public RoomStatus Status { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
