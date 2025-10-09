using System.Text.Json.Serialization;

namespace HotelWebApp.Mobile.Models
{
    public class ConfirmEmailRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}