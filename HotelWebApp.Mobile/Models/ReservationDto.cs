using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Models
{
    public class ReservationDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("checkInDate")]
        public DateTime CheckInDate { get; set; }

        [JsonPropertyName("checkOutDate")]
        public DateTime CheckOutDate { get; set; }

        [JsonPropertyName("reservationDate")]
        public DateTime ReservationDate { get; set; }

        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("numberOfGuests")]
        public int NumberOfGuests { get; set; }

        [JsonPropertyName("room")]
        public RoomDto? Room { get; set; }

        [JsonPropertyName("amenities")]
        public List<ReservationAmenityDto> Amenities { get; set; } = new();

        [JsonPropertyName("originalPrice")]
        public decimal? OriginalPrice { get; set; }

        [JsonPropertyName("discountPercentage")]
        public decimal? DiscountPercentage { get; set; }

        [JsonPropertyName("promotionTitle")]
        public string? PromotionTitle { get; set; }


        // Propriedades calculadas

        public bool HasPromotion => DiscountPercentage.HasValue && DiscountPercentage.Value > 0;
        public string DiscountText => HasPromotion ? $"-{DiscountPercentage:F0}%" : string.Empty;
        public string OriginalPriceFormatted => OriginalPrice.HasValue ? $"€{OriginalPrice:N2}" : string.Empty;
        public string SavingsFormatted => HasPromotion && OriginalPrice.HasValue
            ? $"€{(OriginalPrice.Value - TotalPrice):N2}"
            : string.Empty;

        public string RoomNumber => Room?.RoomNumber ?? "N/A";
        public string RoomType => Room?.Type ?? "N/A";
        public int NumberOfNights => (CheckOutDate.Date - CheckInDate.Date).Days;
        public string CheckInFormatted => CheckInDate.ToString("dd MMM yyyy");
        public string CheckOutFormatted => CheckOutDate.ToString("dd MMM yyyy");
        public string PriceFormatted => $"€{TotalPrice:N2}";

        public string StatusColor => Status?.ToLower() switch
        {
            "confirmed" => "#10B981",
            "pending" => "#F59E0B",
            "cancelled" => "#EF4444",
            "checkedin" => "#3B82F6",
            "completed" => "#6B7280",
            _ => "#6B7280"
        };

        public bool IsCheckedIn => Status?.ToLower() == "checkedin";
        public bool CanCancel => Status?.ToLower() == "confirmed" && !IsCheckedIn
            && CheckInDate.Date > DateTime.Today;
        public bool CanCheckIn => Status?.ToLower() == "confirmed" && !IsCheckedIn
            && CheckInDate.Date == DateTime.Today;
    }
}
