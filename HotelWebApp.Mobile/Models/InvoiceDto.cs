namespace HotelWebApp.Mobile.Models
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public decimal BalanceDue { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public List<ReservationAmenityDto> ReservationAmenities { get; set; } = new();
        public List<PaymentDto> Payments { get; set; } = new();

        // Computed properties for UI
        public bool IsPaid => Status == "Paid";
        public bool HasBalance => BalanceDue > 0;
        public string InvoiceDateFormatted => InvoiceDate.ToString("dd MMM yyyy");
        public string TotalAmountFormatted => $"€{TotalAmount:N2}";
        public string AmountPaidFormatted => $"€{AmountPaid:N2}";
        public string BalanceDueFormatted => $"€{BalanceDue:N2}";

        public string StatusColor => Status switch
        {
            "Paid" => "#10B981",
            "PartiallyPaid" => "#F59E0B",
            "Unpaid" => "#EF4444",
            _ => "#6B7280"
        };

        public string StatusDisplayText => Status switch
        {
            "Paid" => "Paid",
            "PartiallyPaid" => "Partially Paid",
            "Unpaid" => "Unpaid",
            _ => Status
        };

        public string CheckInFormatted => CheckInDate?.ToString("dd MMM yyyy") ?? "N/A";
        public string CheckOutFormatted => CheckOutDate?.ToString("dd MMM yyyy") ?? "N/A";
    }
}