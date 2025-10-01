namespace HotelWebApp.Models.Api
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string? IdentificationDocument { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
