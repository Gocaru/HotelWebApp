using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel representing the data required to create a new employee account.
    /// Used in the CreateEmployee view and action.
    /// </summary>
    public class CreateEmployeeViewModel
    {
        /// <summary>
        /// The full name of the new employee.
        /// </summary>
        [Required]
        [Display(Name = "Full Name")]
        [MaxLength(100)]
        public string FullName { get; set; }

        /// <summary>
        /// The email address for the new employee. This will also serve as their username.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// The initial password for the new employee's account.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string Password { get; set; }


        /// <summary>
        /// A confirmation field for the password to ensure it was typed correctly.
        /// Must match the Password field.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
