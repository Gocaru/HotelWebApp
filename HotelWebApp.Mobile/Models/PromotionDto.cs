using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Models
{
    public class PromotionDto
    {
        [JsonPropertyName("id")] 
        public int PromotionId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("discountPercentage")]
        public decimal DiscountPercentage { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("terms")] 
        public string? Terms { get; set; }


        public string FormattedDiscount => $"{DiscountPercentage}% OFF";

        public string ValidPeriod => $"Valid until {EndDate:MMM dd, yyyy}";

        public string DateRangeFormatted => $"{StartDate:dd MMM} - {EndDate:dd MMM}";

        public bool IsExpired => DateTime.Today > EndDate.Date;

        public bool IsUpcoming => DateTime.Today < StartDate.Date;

        public bool IsCurrentlyActive => IsActive && !IsExpired && !IsUpcoming;

        public string StatusText => IsCurrentlyActive
            ? "ACTIVE"
            : IsExpired
                ? "EXPIRED"
                : "COMING SOON";

        public Color StatusColor => IsCurrentlyActive
            ? Colors.Green
            : IsExpired
                ? Colors.Gray
                : Colors.Orange;
    }
}
