using System.Text.Json.Serialization;

namespace HotelWebApp.Mobile.Models
{
    public class ActivityDto
    {
        [JsonPropertyName("id")]
        public int ActivityId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("duration")]
        public string? Duration { get; set; }

        [JsonPropertyName("schedule")]
        public string? Schedule { get; set; }

        [JsonPropertyName("capacity")]
        public int MaxParticipants { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        // ✅ Propriedades computadas para UI
        public string FormattedPrice => $"€{Price:F2}";
        public string DisplayDuration => Duration ?? "N/A";
        public string DisplaySchedule => Schedule ?? "N/A";

        // ✅ NOVO - Mostrar capacidade diária em vez de disponibilidade
        public string AvailabilityText => MaxParticipants == 1
            ? "1 spot per day"
            : $"Up to {MaxParticipants} daily spots";

        // ✅ Manter para compatibilidade (não usado na lista)
        public bool HasAvailability => true;
    }
}