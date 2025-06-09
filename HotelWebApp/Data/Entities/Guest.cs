using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Data.Entities
{
    public class Guest : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "{0} is required.")]
        [MaxLength(100, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string Name { get; set; }


        [Display(Name = "Contact Number")]
        [Required(ErrorMessage = "{0} is required.")]
        [Phone(ErrorMessage = "Please enter a valid {0}")]
        public string Contact { get; set; }


        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "{0} is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid {0}")]
        public string Email { get; set; }


        [Display(Name = "Identification Document")]
        [Required(ErrorMessage = "{0} is required.")]
        [MaxLength(50, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string IdentificationDocument { get; set; }
    }
}
