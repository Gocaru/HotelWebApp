using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Models
{
    public class ActivityBookingDto
    {
        [JsonPropertyName("id")]
        public int ActivityBookingId { get; set; }

        [JsonPropertyName("activityName")]
        public string ActivityName { get; set; } = string.Empty;

        [JsonPropertyName("bookingDate")]
        public DateTime BookingDate { get; set; }

        [JsonPropertyName("scheduledDate")]
        public DateTime ScheduledDate { get; set; }

        [JsonPropertyName("numberOfPeople")]
        public int NumberOfPeople { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }

        [JsonPropertyName("activityLocation")]
        public string? ActivityLocation { get; set; }

        [JsonPropertyName("activityDuration")]
        public string? ActivityDuration { get; set; }

        [JsonPropertyName("activitySchedule")]
        public string? ActivitySchedule { get; set; }

        [JsonPropertyName("activityPrice")]
        public decimal ActivityPrice { get; set; }

        // Propriedades computadas para UI
        public string FormattedDate => ScheduledDate.ToString("dd/MM/yyyy");

        public string FormattedBookingDate => BookingDate.ToString("dd/MM/yyyy HH:mm");

        // TimeRange usando Duration e Schedule
        public string TimeRange => !string.IsNullOrEmpty(ActivitySchedule)
            ? ActivitySchedule
            : ActivityDuration ?? "N/A";

        public string ParticipantsText => NumberOfPeople == 1
            ? "1 participant"
            : $"{NumberOfPeople} participants";

        public string FormattedTotalPrice => $"€{TotalPrice:F2}";

        public Color StatusColor => Status?.ToLower() switch
        {
            "confirmed" => Colors.Green,
            "cancelled" => Colors.Red,
            "pending" => Colors.Orange,
            "completed" => Colors.Blue,
            _ => Colors.Gray
        };

        public string StatusText => Status;
    }
}
