namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Request model for updating user profile information
    /// </summary>
    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? IdentificationDocument { get; set; }
    }
}
