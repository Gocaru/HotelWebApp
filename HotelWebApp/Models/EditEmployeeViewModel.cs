using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models
{
    public class EditEmployeeViewModel
    {
        public string Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
