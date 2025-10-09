using System.Text.Json.Serialization;

namespace HotelWebApp.Mobile.Models
{
    public class ResendConfirmationRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
}