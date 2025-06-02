using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Data.Entities
{
    public class Guest
    {
        public int GuestId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Contact Number")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Identification document is required.")]
        [MaxLength(ErrorMessage = "Identification document cannot exceed 50 characters.")]
        [Display(Name = "Identification Document")]
        public string IdentificationDocument { get; set; }
    }
}
