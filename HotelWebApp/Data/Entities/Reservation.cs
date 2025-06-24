using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    public class Reservation : IEntity
    {
        public int Id { get ; set ; }

        [Required(ErrorMessage ="Check-in date is required.")]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set ; }

        [Required(ErrorMessage = "Check-out date is required.")]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Reservation Date")]
        public DateTime ReservationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Total price is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Price")]
        public decimal TotalPrice {  get; set ; }

        [Required]
        [Display(Name = "Status")]
        public ReservationStatus Status { get; set ; }

        [Required]
        public int RoomId { get; set ; }

        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set ; }

        [Required]
        public string GuestId { get; set ; }

        [ForeignKey("GuestId")]
        public virtual ApplicationUser? Guest { get; set ; }
    }
}
