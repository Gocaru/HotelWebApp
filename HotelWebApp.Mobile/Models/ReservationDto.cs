using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Models
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime ReservationDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public int NumberOfGuests { get; set; }
        public RoomDto? Room { get; set; }
        public List<ReservationAmenityDto> Amenities { get; set; } = new();

        // Propriedades calculadas
        public string RoomNumber => Room?.RoomNumber ?? "N/A";
        public string RoomType => Room?.Type ?? "N/A";
        public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
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
