using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel representing the data that can be edited for an employee account.
    /// Used in the Edit Employee view and action.
    /// </summary>
    public class EditEmployeeViewModel
    {
        /// <summary>
        /// The unique ID of the employee user being edited.
        /// This is typically passed as a hidden field in the form.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The full name of the employee.
        /// </summary>
        [Required]
        public string FullName { get; set; }

        /// <summary>
        /// The email address of the employee. Changing this will also update their username.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
