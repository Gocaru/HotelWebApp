using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Request model for initiating password reset process
    /// </summary>
    public class MobileForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string AppUrl { get; set; } = string.Empty;
    }
}
