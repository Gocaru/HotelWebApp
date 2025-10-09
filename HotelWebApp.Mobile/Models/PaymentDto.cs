namespace HotelWebApp.Mobile.Models
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionId { get; set; }

        public string PaymentDateFormatted => PaymentDate.ToString("dd MMM yyyy HH:mm");
        public string AmountFormatted => $"€{Amount:N2}";
    }
}