using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Request model for changing user password while authenticated
    /// </summary>
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Current password is required")]
        [JsonPropertyName("currentPassword")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
