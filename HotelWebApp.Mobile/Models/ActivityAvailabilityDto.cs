using System.Text.Json.Serialization;

namespace HotelWebApp.Mobile.Models
{
    public class ActivityAvailabilityDto
    {
        [JsonPropertyName("activityId")]
        public int ActivityId { get; set; }

        [JsonPropertyName("activityName")]
        public string ActivityName { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }

        [JsonPropertyName("currentParticipants")]
        public int CurrentParticipants { get; set; }

        [JsonPropertyName("availableSpots")]
        public int AvailableSpots { get; set; }

        [JsonPropertyName("isFull")]
        public bool IsFull { get; set; }

        // ✅ Propriedades computadas para UI
        public string AvailabilityText => IsFull
            ? "Fully Booked"
            : $"{AvailableSpots} spots available";

        public Color AvailabilityColor => IsFull
            ? Colors.Red
            : AvailableSpots <= 2
                ? Colors.Orange
                : Colors.Green;
    }
}