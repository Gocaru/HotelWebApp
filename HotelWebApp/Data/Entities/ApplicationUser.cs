using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents a user in the application, extending the built-in ASP.NET Core IdentityUser.
    /// This class adds custom properties relevant to the hotel management system, such as FullName and profile picture.
    /// </summary>
    public class ApplicationUser: IdentityUser
    {
        /// <summary>
        /// The full name of the user (e.g., "John Employee", "Mary Guest").
        /// </summary>
        [Required(ErrorMessage = "Full Name is required.")]
        [MaxLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// The file name of the user's profile picture.
        /// The file itself is stored in the wwwroot/images/profiles directory.
        /// This property can be null if the user has not uploaded a picture.
        /// </summary>
        [Display(Name = "Profile Picture")]
        public string? ProfilePictureUrl { get; set; }

        /// <summary>
        /// An optional identification document number for the user, such as a passport or national ID.
        /// This is typically required for guests at check-in.
        /// </summary>
        [MaxLength(50, ErrorMessage = "Identification Document cannot exceed 50 characters.")]
        [Display(Name = "Identification Document")]
        public string? IdentificationDocument { get; set; }

        /// <summary>
        /// Navigation property representing all reservations made by this user (if they are a Guest).
        /// </summary>
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        /// <summary>
        /// Navigation property representing all invoices issued to this user (if they are a Guest).
        /// </summary>
        public virtual ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();

    }
}
