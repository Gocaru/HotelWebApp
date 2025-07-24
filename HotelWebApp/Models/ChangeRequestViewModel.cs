using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models
{
    public class ChangeRequestViewModel
    {
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "Please describe the changes you would like to request.")]
        [Display(Name = "Your Request")]
        [StringLength(500, ErrorMessage = "The request details cannot exceed 500 characters.")]
        public string RequestDetails { get; set; }
    }
}
