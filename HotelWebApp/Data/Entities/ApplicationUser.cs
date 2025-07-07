using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Data.Entities
{
    public class ApplicationUser: IdentityUser
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [MaxLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Profile Picture")]
        public string? ProfilePictureUrl { get; set; }

        [MaxLength(50, ErrorMessage = "Identification Document cannot exceed 50 characters.")]
        [Display(Name = "Identification Document")]
        public string? IdentificationDocument { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public virtual ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();

        ///// <summary>
        ///// Constructor to initialize collections and prevent null reference exceptions.
        ///// </summary>
        //public ApplicationUser()
        //{
        //    Reservations = new HashSet<Reservation>();
        //    Invoices = new HashSet<Invoice>();
        //}
    }
}
