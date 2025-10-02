namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Represents a financial invoice generated for a reservation
    /// </summary>
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
    }
}
