using HotelWebApp.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models
{
    public class ReservationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a guest.")]
        [Display(Name = "Guest")]
        public string GuestId { get; set; } // O ID do ApplicationUser

        [Required(ErrorMessage ="Please select a room.")]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        public ReservationStatus? Status { get; set; }

        public IEnumerable<SelectListItem>? Guests { get; set; }
        public IEnumerable<SelectListItem>? Rooms { get; set; }


    }
}
