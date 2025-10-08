using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Models
{
    public class UserDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("profilePictureUrl")]
        public string? ProfilePictureUrl { get; set; }

        [JsonPropertyName("identificationDocument")]
        public string? IdentificationDocument { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        // Propriedades adicionais para estatísticas (calculadas no mobile)
        public int TotalReservations { get; set; }
        public int CompletedStays { get; set; }
        public int UpcomingReservations { get; set; }

        // Computed properties
        public string DisplayPhoneNumber => string.IsNullOrEmpty(PhoneNumber) ? "Not provided" : PhoneNumber;
        public string DisplayDocument => string.IsNullOrEmpty(IdentificationDocument) ? "Not provided" : IdentificationDocument;
        public bool HasProfilePicture => !string.IsNullOrEmpty(ProfilePictureUrl);

        public string ProfileImageSource => HasProfilePicture
            ? $"{Helpers.Constants.ApiBaseUrl}images/profiles/{ProfilePictureUrl}?v={Guid.NewGuid()}"
            : "user_placeholder.png";
    }
}
