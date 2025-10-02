using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Request model for changing user password while authenticated
    /// </summary>
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
